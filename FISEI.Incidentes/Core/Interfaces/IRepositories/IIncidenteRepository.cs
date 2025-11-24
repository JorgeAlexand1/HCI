using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    /// <summary>
    /// Repositorio especializado para Incidentes (ITIL Incident Management)
    /// </summary>
    public interface IIncidenteRepository : IRepository<Incidente>
    {
        Task<IEnumerable<Incidente>> GetIncidentesPorEstadoAsync(int idEstado);
        Task<IEnumerable<Incidente>> GetIncidentesPorUsuarioAsync(int idUsuario);
        Task<IEnumerable<Incidente>> GetIncidentesPorNivelAsync(int idNivelSoporte);
        Task<IEnumerable<Incidente>> GetIncidentesSinAsignarAsync();
        Task<IEnumerable<Incidente>> GetIncidentesRecurrentesAsync(int idCategoria, int minOcurrencias);
        Task<int> ContarIncidentesPorTecnicoAsync(int idTecnico);
    }
}