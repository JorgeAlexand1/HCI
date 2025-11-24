using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    public interface IConocimientoRepository : IRepository<Conocimiento>
    {
        Task<IEnumerable<Conocimiento>> BuscarPorPalabrasClave(string palabrasClave);
        Task<IEnumerable<Conocimiento>> GetArticulosAprobadosAsync();
        Task IncrementarVisualizacionesAsync(int idConocimiento);
    }
}
