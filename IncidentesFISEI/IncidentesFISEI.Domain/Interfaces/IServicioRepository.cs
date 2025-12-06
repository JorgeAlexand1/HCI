using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Domain.Interfaces;

public interface IServicioRepository : IRepository<Servicio>
{
    Task<IEnumerable<Servicio>> GetByCategoriaIdAsync(int categoriaId);
    Task<IEnumerable<Servicio>> GetActiveServiciosAsync();
    Task<Servicio?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Servicio>> SearchServiciosAsync(string searchTerm);
    Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null);
}