using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces;

public interface IServicioService
{
    Task<IEnumerable<ServicioListDto>> GetAllServiciosAsync();
    Task<IEnumerable<ServicioListDto>> GetServiciosByCategoriaAsync(int categoriaId);
    Task<ServicioDto?> GetServicioByIdAsync(int id);
    Task<ServicioDto> CreateServicioAsync(CreateServicioDto createDto);
    Task<ServicioDto?> UpdateServicioAsync(int id, UpdateServicioDto updateDto);
    Task<bool> DeleteServicioAsync(int id);
    Task<IEnumerable<ServicioListDto>> SearchServiciosAsync(string searchTerm);
    Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null);
}