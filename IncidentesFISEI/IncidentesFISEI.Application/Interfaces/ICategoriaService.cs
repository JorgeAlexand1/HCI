using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaIncidenteDto>> GetAllCategoriasAsync();
    Task<CategoriaIncidenteDto> GetCategoriaByIdAsync(int id);
    Task<CategoriaIncidenteDto> CreateCategoriaAsync(CreateCategoriaIncidenteDto createCategoriaDto);
    Task<CategoriaIncidenteDto> UpdateCategoriaAsync(int id, UpdateCategoriaIncidenteDto updateCategoriaDto);
    Task<bool> DeleteCategoriaAsync(int id);
    Task<IEnumerable<CategoriaIncidenteDto>> GetCategoriasActivasAsync();
}