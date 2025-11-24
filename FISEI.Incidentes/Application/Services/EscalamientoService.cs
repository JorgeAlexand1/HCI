using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;
using FISEI.Incidentes.Infrastructure.Data;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FISEI.Incidentes.Application.Services
{
    /// <summary>
    /// Servicio de escalamiento según ITIL v3
    /// Implementa escalamiento funcional (Técnicos → Expertos → Proveedores)
    /// </summary>
    public class EscalamientoService : IEscalamientoService
    {
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IAsignacionService _asignacionService;
        private readonly INotificacionService _notificacionService;
        private readonly ApplicationDbContext _context;

        // Tiempo máximo en horas por nivel antes de escalar
        private readonly Dictionary<int, int> _tiemposEscalamientoHoras = new()
        {
            { 1, 24 },  // N1: 24 horas
            { 2, 48 },  // N2: 48 horas
            { 3, 72 }   // N3: 72 horas
        };

        public EscalamientoService(
            IIncidenteRepository incidenteRepository,
            IAsignacionService asignacionService,
            INotificacionService notificacionService,
            ApplicationDbContext context)
        {
            _incidenteRepository = incidenteRepository;
            _asignacionService = asignacionService;
            _notificacionService = notificacionService;
            _context = context;
        }

        /// <summary>
        /// Escala un incidente al siguiente nivel de soporte
        /// </summary>
        public async Task<Incidente> EscalarIncidenteAsync(int idIncidente, string motivoEscalamiento)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            int nivelAnterior = incidente.IdNivelSoporte;

            // Verificar si puede escalar (máximo N3)
            if (incidente.IdNivelSoporte >= 3)
                throw new Exception("El incidente ya está en el nivel máximo de soporte");

            // Incrementar nivel
            incidente.IdNivelSoporte++;
            await _incidenteRepository.UpdateAsync(incidente);

            // Registrar en historial
            var historial = new Historial
            {
                IdIncidente = idIncidente,
                IdUsuario = 1, // Sistema (deberías obtener el usuario actual)
                Fecha = DateTime.Now,
                Comentario = $"Escalamiento de N{nivelAnterior} a N{incidente.IdNivelSoporte}. Motivo: {motivoEscalamiento}"
            };
            await _context.HistorialesIncidentes.AddAsync(historial);
            await _context.SaveChangesAsync();

            // Reasignar automáticamente al nuevo nivel
            await _asignacionService.AsignarAutomaticamenteAsync(idIncidente);

            // Notificar escalamiento
            await _notificacionService.NotificarEscalamientoAsync(idIncidente, nivelAnterior, incidente.IdNivelSoporte);

            return incidente;
        }

        /// <summary>
        /// Verifica si un incidente debe escalar por tiempo transcurrido
        /// </summary>
        public async Task<bool> DebeEscalarPorTiempoAsync(int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return false;

            // No escalar si ya está cerrado o en nivel máximo
            if (incidente.FechaCierre != null || incidente.IdNivelSoporte >= 3)
                return false;

            var tiempoLimite = _tiemposEscalamientoHoras[incidente.IdNivelSoporte];
            var horasTranscurridas = (DateTime.Now - incidente.FechaCreacion).TotalHours;

            return horasTranscurridas > tiempoLimite;
        }

        /// <summary>
        /// Verifica si un incidente es recurrente y debe escalar (ITIL Problem Management)
        /// </summary>
        public async Task<bool> DebeEscalarPorRecurrenciaAsync(int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return false;

            // Buscar incidentes similares en los últimos 30 días
            var incidentesRecurrentes = await _incidenteRepository.GetIncidentesRecurrentesAsync(
                incidente.IdCategoria, 
                minOcurrencias: 3
            );

            return incidentesRecurrentes.Any();
        }

        /// <summary>
        /// Proceso automático de escalamiento (ejecutar con tarea en segundo plano)
        /// </summary>
        public async Task EscalarIncidentesAutomaticamenteAsync()
        {
            // Obtener todos los incidentes abiertos
            var estadoAbierto = await _context.EstadosIncidentes
                .FirstOrDefaultAsync(e => e.Nombre == "Abierto");
            
            if (estadoAbierto == null)
                return;

            var incidentesAbiertos = await _incidenteRepository.GetIncidentesPorEstadoAsync(estadoAbierto.IdEstado);

            foreach (var incidente in incidentesAbiertos)
            {
                try
                {
                    // Verificar escalamiento por tiempo
                    if (await DebeEscalarPorTiempoAsync(incidente.IdIncidente))
                    {
                        await EscalarIncidenteAsync(
                            incidente.IdIncidente, 
                            "Escalamiento automático por tiempo de resolución excedido"
                        );
                    }

                    // Verificar escalamiento por recurrencia
                    if (await DebeEscalarPorRecurrenciaAsync(incidente.IdIncidente))
                    {
                        await EscalarIncidenteAsync(
                            incidente.IdIncidente, 
                            "Escalamiento automático por incidente recurrente (posible problema)"
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Log del error (implementar logging)
                    Console.WriteLine($"Error al escalar incidente {incidente.IdIncidente}: {ex.Message}");
                }
            }
        }
    }
}