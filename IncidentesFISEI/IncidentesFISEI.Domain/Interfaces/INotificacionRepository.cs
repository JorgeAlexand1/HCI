using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Interfaces;

public interface INotificacionRepository : IRepository<Notificacion>
{
    Task<IEnumerable<Notificacion>> GetNotificacionesByUsuarioAsync(int usuarioId, bool soloNoLeidas = false);
    Task<int> GetCountNoLeidasAsync(int usuarioId);
    Task MarcarComoLeidaAsync(int notificacionId);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
    Task<IEnumerable<Notificacion>> GetNotificacionesPorTipoAsync(int usuarioId, TipoNotificacion tipo);
    Task<IEnumerable<Notificacion>> GetNotificacionesPorIncidenteAsync(int incidenteId);
    Task EliminarNotificacionesAntiguasAsync(DateTime fechaLimite);
}
