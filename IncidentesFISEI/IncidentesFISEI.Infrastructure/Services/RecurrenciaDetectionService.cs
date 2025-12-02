using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services
{
    /// <summary>
    /// Servicio en segundo plano que detecta incidentes recurrentes
    /// Analiza patrones cada 30 minutos para identificar problemas sistémicos según ITIL
    /// </summary>
    public class RecurrenciaDetectionService : BackgroundService
    {
        private readonly ILogger<RecurrenciaDetectionService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(30);

        // Umbrales de detección
        private const int UmbralRecurrenciaMismoUsuario = 3; // 3+ incidentes del mismo usuario en 7 días
        private const int UmbralRecurrenciaMismaCategoria = 5; // 5+ incidentes en misma categoría en 24 horas
        private const int DiasAnalisisMismoUsuario = 7;
        private const int HorasAnalisisMismaCategoria = 24;

        public RecurrenciaDetectionService(
            ILogger<RecurrenciaDetectionService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 Recurrencia Detection Service iniciado. Intervalo: {Intervalo} minutos",
                _intervalo.TotalMinutes);

            // Esperar 5 minutos antes de la primera ejecución
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DetectarRecurrenciasAsync();
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Recurrencia Detection Service detenido");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en Recurrencia Detection Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task DetectarRecurrenciasAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var incidenteRepo = scope.ServiceProvider.GetRequiredService<IIncidenteRepository>();
            var comentarioRepo = scope.ServiceProvider.GetRequiredService<IComentarioRepository>();

            var ahora = DateTime.UtcNow;
            var incidentes = (await incidenteRepo.GetAllAsync()).ToList();

            // Detección 1: Mismo usuario reportando múltiples incidentes
            var recurrenciasPorUsuario = await DetectarRecurrenciasPorUsuarioAsync(
                incidentes, ahora, comentarioRepo);

            // Detección 2: Múltiples incidentes en la misma categoría
            var recurrenciasPorCategoria = await DetectarRecurrenciasPorCategoriaAsync(
                incidentes, ahora, comentarioRepo);

            // Detección 3: Incidentes con palabras clave similares
            var recurrenciasPorPatron = await DetectarRecurrenciasPorPatronAsync(
                incidentes, ahora, comentarioRepo);

            var totalRecurrencias = recurrenciasPorUsuario + recurrenciasPorCategoria + recurrenciasPorPatron;

            if (totalRecurrencias > 0)
            {
                _logger.LogWarning("🔄 Detectadas {Total} recurrencias - Usuario: {Usuario}, Categoría: {Categoria}, Patrón: {Patron}",
                    totalRecurrencias, recurrenciasPorUsuario, recurrenciasPorCategoria, recurrenciasPorPatron);
            }
            else
            {
                _logger.LogInformation("✅ Análisis de recurrencias completado: 0 patrones detectados");
            }
        }

        private async Task<int> DetectarRecurrenciasPorUsuarioAsync(
            List<Incidente> incidentes,
            DateTime ahora,
            IComentarioRepository comentarioRepo)
        {
            var fechaLimite = ahora.AddDays(-DiasAnalisisMismoUsuario);
            var incidentesRecientes = incidentes
                .Where(i => i.FechaReporte >= fechaLimite)
                .GroupBy(i => i.ReportadoPorId)
                .Where(g => g.Count() >= UmbralRecurrenciaMismoUsuario)
                .ToList();

            foreach (var grupo in incidentesRecientes)
            {
                var incidentesUsuario = grupo.ToList();
                var categoriasMasComunes = incidentesUsuario
                    .GroupBy(i => i.CategoriaId)
                    .OrderByDescending(g => g.Count())
                    .First();

                _logger.LogWarning(
                    "🔄 Usuario {UsuarioId} reportó {Cantidad} incidentes en {Dias} días. Categoría más común: {CategoriaId}",
                    grupo.Key, incidentesUsuario.Count, DiasAnalisisMismoUsuario, categoriasMasComunes.Key);

                // Agregar comentario automático al incidente más reciente
                var incidenteMasReciente = incidentesUsuario.OrderByDescending(i => i.FechaReporte).First();
                await AgregarComentarioRecurrenciaAsync(
                    incidenteMasReciente,
                    $"🔄 PATRÓN DETECTADO: Usuario con {incidentesUsuario.Count} incidentes similares en {DiasAnalisisMismoUsuario} días. " +
                    $"Se recomienda análisis de causa raíz y capacitación.",
                    comentarioRepo);
            }

            return incidentesRecientes.Count;
        }

        private async Task<int> DetectarRecurrenciasPorCategoriaAsync(
            List<Incidente> incidentes,
            DateTime ahora,
            IComentarioRepository comentarioRepo)
        {
            var fechaLimite = ahora.AddHours(-HorasAnalisisMismaCategoria);
            var incidentesRecientes = incidentes
                .Where(i => i.FechaReporte >= fechaLimite && 
                            i.Estado != EstadoIncidente.Cerrado)
                .GroupBy(i => i.CategoriaId)
                .Where(g => g.Count() >= UmbralRecurrenciaMismaCategoria)
                .ToList();

            foreach (var grupo in incidentesRecientes)
            {
                var incidentesCategoria = grupo.ToList();
                
                _logger.LogWarning(
                    "🔄 Categoría {CategoriaId}: {Cantidad} incidentes activos en {Horas} horas. Posible problema sistémico.",
                    grupo.Key, incidentesCategoria.Count, HorasAnalisisMismaCategoria);

                // Marcar todos los incidentes de este grupo como posiblemente relacionados
                foreach (var incidente in incidentesCategoria)
                {
                    await AgregarComentarioRecurrenciaAsync(
                        incidente,
                        $"⚠️ ALERTA: {incidentesCategoria.Count} incidentes en esta categoría en las últimas {HorasAnalisisMismaCategoria}h. " +
                        $"Posible problema sistémico. Se recomienda escalación a Nivel 3.",
                        comentarioRepo);
                }
            }

            return incidentesRecientes.Count;
        }

        private async Task<int> DetectarRecurrenciasPorPatronAsync(
            List<Incidente> incidentes,
            DateTime ahora,
            IComentarioRepository comentarioRepo)
        {
            // Analizar últimos 3 días
            var fechaLimite = ahora.AddDays(-3);
            var incidentesRecientes = incidentes
                .Where(i => i.FechaReporte >= fechaLimite)
                .ToList();

            var palabrasClave = new[] { "error", "conexión", "red", "internet", "lento", "caído", "no funciona", "contraseña" };
            int patronesDetectados = 0;

            foreach (var palabra in palabrasClave)
            {
                var incidentesConPalabra = incidentesRecientes
                    .Where(i => i.Titulo.Contains(palabra, StringComparison.OrdinalIgnoreCase) ||
                                i.Descripcion.Contains(palabra, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (incidentesConPalabra.Count >= 4) // 4+ incidentes con misma palabra clave
                {
                    _logger.LogWarning(
                        "🔄 Patrón detectado: {Cantidad} incidentes contienen '{Palabra}' en los últimos 3 días",
                        incidentesConPalabra.Count, palabra);

                    patronesDetectados++;

                    // Solo comentar en el más reciente para no saturar
                    var incidenteMasReciente = incidentesConPalabra.OrderByDescending(i => i.FechaReporte).First();
                    await AgregarComentarioRecurrenciaAsync(
                        incidenteMasReciente,
                        $"🔍 PATRÓN: {incidentesConPalabra.Count} incidentes relacionados con '{palabra}' detectados. " +
                        $"Considerar creación de artículo de conocimiento.",
                        comentarioRepo);
                }
            }

            return patronesDetectados;
        }

        private async Task AgregarComentarioRecurrenciaAsync(
            Incidente incidente,
            string mensaje,
            IComentarioRepository comentarioRepo)
        {
            // Verificar que no exista ya un comentario similar reciente
            var comentariosExistentes = await comentarioRepo.FindAsync(c =>
                c.IncidenteId == incidente.Id &&
                c.CreatedAt > DateTime.UtcNow.AddHours(-6) &&
                c.Contenido.Contains("PATRÓN") || c.Contenido.Contains("ALERTA"));

            if (comentariosExistentes.Any())
                return; // Ya existe un comentario de recurrencia reciente

            var comentario = new ComentarioIncidente
            {
                IncidenteId = incidente.Id,
                AutorId = 1, // Sistema (Admin)
                Contenido = mensaje,
                Tipo = TipoComentario.ActualizacionEstado,
                EsInterno = true,
                CreatedAt = DateTime.UtcNow
            };

            await comentarioRepo.AddAsync(comentario);
            await comentarioRepo.SaveChangesAsync();
        }
    }
}
