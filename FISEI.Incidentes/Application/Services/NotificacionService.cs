using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.SignalR;
using FISEI.Incidentes.Presentation.Hubs;

namespace FISEI.Incidentes.Application.Services
{
    /// <summary>
    /// Servicio de notificaciones en tiempo real con SignalR
    /// </summary>
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificacionService(
            INotificacionRepository notificacionRepository,
            IIncidenteRepository incidenteRepository,
            IHubContext<NotificationHub> hubContext)
        {
            _notificacionRepository = notificacionRepository;
            _incidenteRepository = incidenteRepository;
            _hubContext = hubContext;
        }

        public async Task EnviarNotificacionAsync(int idUsuario, string mensaje, string tipo)
        {
            var notificacion = new Notificacion
            {
                IdUsuarioDestino = idUsuario,
                Mensaje = mensaje,
                FechaEnvio = DateTime.Now,
                Leido = false
            };

            await _notificacionRepository.AddAsync(notificacion);

            // Enviar notificación push en tiempo real con SignalR
            await _hubContext.Clients.Group($"user_{idUsuario}")
                .SendAsync("ReceiveNotification", new
                {
                    id = notificacion.IdNotificacion,
                    mensaje = mensaje,
                    tipo = tipo,
                    fechaEnvio = notificacion.FechaEnvio
                });
        }

        public async Task NotificarNuevoIncidenteAsync(int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return;

            // Notificar al usuario que creó el incidente
            await EnviarNotificacionAsync(
                incidente.IdUsuario,
                $"Tu incidente #{idIncidente} - '{incidente.Titulo}' ha sido registrado exitosamente",
                "info"
            );
        }

        public async Task NotificarAsignacionAsync(int idTecnico, int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return;

            await EnviarNotificacionAsync(
                idTecnico,
                $"Se te ha asignado el incidente #{idIncidente} - '{incidente.Titulo}'",
                "assignment"
            );
        }

        public async Task NotificarEscalamientoAsync(int idIncidente, int nivelAnterior, int nivelNuevo)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return;

            // Notificar al usuario que report�
            await EnviarNotificacionAsync(
                incidente.IdUsuario,
                $"Tu incidente #{idIncidente} ha sido escalado de N{nivelAnterior} a N{nivelNuevo}",
                "escalation"
            );
        }

        public async Task NotificarCambioEstadoAsync(int idIncidente, int idEstadoAnterior, int idEstadoNuevo)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return;

            await EnviarNotificacionAsync(
                incidente.IdUsuario,
                $"El estado de tu incidente #{idIncidente} ha cambiado a '{incidente.Estado.Nombre}'",
                "status_change"
            );
        }

        public async Task MarcarComoLeidaAsync(int idNotificacion)
        {
            await _notificacionRepository.MarcarComoLeidaAsync(idNotificacion);
        }

        public async Task<IEnumerable<Notificacion>> ObtenerNotificacionesNoLeidasAsync(int idUsuario)
        {
            return await _notificacionRepository.GetNotificacionesNoLeidasAsync(idUsuario);
        }
    }
}