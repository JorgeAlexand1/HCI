using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    public interface IRolRepository : IRepository<Rol>
    {
        Task<Rol?> GetByNombreAsync(string nombre);
        Task<IEnumerable<Rol>> GetRolesDisponiblesParaAsignacionAsync();
    }
}
