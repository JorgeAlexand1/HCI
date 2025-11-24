using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    public interface IAsignacionRepository : IRepository<Asignacion>
    {
        Task<Asignacion?> GetAsignacionActivaPorIncidenteAsync(int idIncidente);
        Task<IEnumerable<Asignacion>> GetAsignacionesPorTecnicoAsync(int idTecnico);
        Task DesactivarAsignacionesAnterioresAsync(int idIncidente);
    }
}
