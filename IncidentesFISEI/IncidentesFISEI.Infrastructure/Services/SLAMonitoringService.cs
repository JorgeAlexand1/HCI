using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services
{
    /// <summary>
    /// Servicio en segundo plano que monitorea violaciones de SLA
    /// Ejecuta verificaciones cada 5 minutos según ITIL v4
    /// </summary>
    public class SLAMonitoringService : BackgroundService
    {
        private readonly ILogger<SLAMonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(5);

        public SLAMonitoringService(
            ILogger<SLAMonitoringService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 SLA Monitoring Service iniciado. Intervalo: {Intervalo} minutos", 
                _intervalo.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorearSLAsAsync();
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("SLA Monitoring Service detenido");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en SLA Monitoring Service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task MonitorearSLAsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var incidenteRepo = scope.ServiceProvider.GetRequiredService<IIncidenteRepository>();
            var slaRepo = scope.ServiceProvider.GetRequiredService<IRepository<SLA>>();
            var escalacionSLARepo = scope.ServiceProvider.GetRequiredService<IRepository<EscalacionSLA>>();

            var incidentesActivos = (await incidenteRepo.GetAllAsync())
                .Where(i => i.Estado != EstadoIncidente.Cerrado && 
                            i.Estado != EstadoIncidente.Resuelto &&
                            i.Estado != EstadoIncidente.Cancelado)
                .ToList();

            var slas = (await slaRepo.GetAllAsync()).ToList();
            int violacionesDetectadas = 0;

            foreach (var incidente in incidentesActivos)
            {
                // Buscar SLA aplicable
                var sla = slas.FirstOrDefault(s => 
                    s.Prioridad == incidente.Prioridad &&
                    s.Impacto == incidente.Impacto &&
                    s.Urgencia == incidente.Urgencia);

                if (sla == null) continue;

                var ahora = DateTime.UtcNow;
                
                // Verificar violación de tiempo de respuesta
                if (!incidente.FechaAsignacion.HasValue)
                {
                    var minutosDesdeReporte = (ahora - incidente.FechaReporte).TotalMinutes;
                    if (minutosDesdeReporte > sla.TiempoRespuesta)
                    {
                        await RegistrarViolacionSLAAsync(
                            incidente, 
                            sla, 
                            $"Violación de tiempo de respuesta: {minutosDesdeReporte:F0} min > {sla.TiempoRespuesta} min",
                            escalacionSLARepo);
                        violacionesDetectadas++;
                    }
                }
                
                // Verificar violación de tiempo de resolución
                if (!incidente.FechaResolucion.HasValue)
                {
                    var minutosDesdeAsignacion = incidente.FechaAsignacion.HasValue
                        ? (ahora - incidente.FechaAsignacion.Value).TotalMinutes
                        : (ahora - incidente.FechaReporte).TotalMinutes;

                    if (minutosDesdeAsignacion > sla.TiempoResolucion)
                    {
                        await RegistrarViolacionSLAAsync(
                            incidente,
                            sla,
                            $"Violación de tiempo de resolución: {minutosDesdeAsignacion:F0} min > {sla.TiempoResolucion} min",
                            escalacionSLARepo);
                        violacionesDetectadas++;
                    }
                }

                // Actualizar fecha de vencimiento si no está configurada
                if (!incidente.FechaVencimiento.HasValue && incidente.FechaAsignacion.HasValue)
                {
                    incidente.FechaVencimiento = incidente.FechaAsignacion.Value
                        .AddMinutes(sla.TiempoResolucion);
                    await incidenteRepo.UpdateAsync(incidente);
                }
            }

            if (violacionesDetectadas > 0)
            {
                _logger.LogWarning("⚠️ Se detectaron {Cantidad} violaciones de SLA de {Total} incidentes activos",
                    violacionesDetectadas, incidentesActivos.Count);
            }
            else
            {
                _logger.LogInformation("✅ Monitoreo SLA completado: {Total} incidentes verificados, 0 violaciones",
                    incidentesActivos.Count);
            }
        }

        private async Task RegistrarViolacionSLAAsync(
            Incidente incidente,
            SLA sla,
            string motivo,
            IRepository<EscalacionSLA> escalacionRepo)
        {
            // Verificar si ya existe una escalación reciente
            var escalacionesExistentes = await escalacionRepo.FindAsync(e => 
                e.IncidenteId == incidente.Id &&
                e.FechaEscalacion > DateTime.UtcNow.AddHours(-1));

            if (escalacionesExistentes.Any())
                return; // Ya se registró recientemente

            var escalacion = new EscalacionSLA
            {
                IncidenteId = incidente.Id,
                SLAId = sla.Id,
                Motivo = motivo,
                FechaEscalacion = DateTime.UtcNow
            };

            await escalacionRepo.AddAsync(escalacion);
            await escalacionRepo.SaveChangesAsync();

            _logger.LogWarning("⚠️ SLA violado - Incidente: {NumeroIncidente}, Motivo: {Motivo}",
                incidente.NumeroIncidente, motivo);
        }
    }
}
