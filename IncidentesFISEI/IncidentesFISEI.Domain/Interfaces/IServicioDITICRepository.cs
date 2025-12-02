using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Domain.Interfaces
{
    public interface IServicioDITICRepository : IRepository<ServicioDITIC>
    {
        Task<IEnumerable<ServicioDITIC>> GetServiciosActivosAsync();
        Task<IEnumerable<ServicioDITIC>> GetServiciosPorTipoAsync(int tipoServicio);
        Task<ServicioDITIC?> GetServicioPorCodigoAsync(string codigo);
        Task<IEnumerable<ServicioDITIC>> GetServiciosEsencialesAsync();
    }
}
