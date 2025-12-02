using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IBaseConocimientoService
{
    Task<IEnumerable<ArticuloConocimientoDto>> GetAllArticulosAsync();
    Task<ArticuloConocimientoDto> GetArticuloByIdAsync(int id);
    Task<IEnumerable<ArticuloConocimientoDto>> SearchArticulosAsync(string searchTerm);
    Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosByTipoAsync(TipoArticulo tipo);
    Task<ArticuloConocimientoDto> CreateArticuloAsync(CreateArticuloConocimientoDto createArticuloDto);
    Task<ArticuloConocimientoDto> UpdateArticuloAsync(int id, UpdateArticuloConocimientoDto updateArticuloDto);
    Task<bool> DeleteArticuloAsync(int id);
    Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosPublicosAsync();
}