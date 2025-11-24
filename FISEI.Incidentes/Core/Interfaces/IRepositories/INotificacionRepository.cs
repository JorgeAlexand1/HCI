using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    public interface INotificacionRepository : IRepository<Notificacion>
    {
        Task<IEnumerable<Notificacion>> GetNotificacionesNoLeidasAsync(int idUsuario);
        Task MarcarComoLeidaAsync(int idNotificacion);
        Task MarcarTodasComoLeidasAsync(int idUsuario);
    }
}
