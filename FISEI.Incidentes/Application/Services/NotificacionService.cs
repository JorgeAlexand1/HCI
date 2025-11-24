using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;

namespace FISEI.Incidentes.Application.Services
{
    /// <summary>
    /// Servicio de notificaciones en tiempo real
    /// </summary>
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IIncidenteRepository _incidenteRepository;
        // TODO: Agregar IHubContext<NotificationHub> para SignalR

        public NotificacionService(
            INotificacionRepository notificacionRepository,
            IIncidenteRepository incidenteRepository)
        {
            _notificacionRepository = notificacionRepository;
            _incidenteRepository = incidenteRepository;
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

            // TODO: Enviar notificación en tiempo real con SignalR
            // await _hubContext.Clients.User(idUsuario.ToString()).SendAsync("ReceiveNotification", mensaje, tipo);
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

            // TODO: Notificar al SPOC
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

            // Notificar al usuario que reportó
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