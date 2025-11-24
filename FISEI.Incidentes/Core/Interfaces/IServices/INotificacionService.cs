using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IServices
{
    /// <summary>
    /// Servicio de notificaciones en tiempo real (SignalR)
    /// </summary>
    public interface INotificacionService
    {
        Task EnviarNotificacionAsync(int idUsuario, string mensaje, string tipo);
        Task NotificarNuevoIncidenteAsync(int idIncidente);
        Task NotificarAsignacionAsync(int idTecnico, int idIncidente);
        Task NotificarEscalamientoAsync(int idIncidente, int nivelAnterior, int nivelNuevo);
        Task NotificarCambioEstadoAsync(int idIncidente, int idEstadoAnterior, int idEstadoNuevo);
        Task MarcarComoLeidaAsync(int idNotificacion);
        Task<IEnumerable<Notificacion>> ObtenerNotificacionesNoLeidasAsync(int idUsuario);
    }
}