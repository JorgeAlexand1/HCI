using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IIncidenteService
{
    Task<ApiResponse<IncidenteDto>> CreateIncidenteAsync(CreateIncidenteDto createDto, int reportadoPorId);
    Task<ApiResponse<IncidenteDto>> UpdateIncidenteAsync(UpdateIncidenteDto updateDto, int updatedById);
    Task<ApiResponse<IncidenteDetalleDto>> GetIncidenteByIdAsync(int id);
    Task<ApiResponse<IEnumerable<IncidenteDto>>> GetIncidentesAsync();
    Task<ApiResponse<IEnumerable<IncidenteDto>>> GetIncidentesByUsuarioAsync(int usuarioId);
    Task<ApiResponse<IEnumerable<IncidenteDto>>> GetIncidentesByTecnicoAsync(int tecnicoId);
    Task<ApiResponse<IEnumerable<IncidenteDto>>> GetIncidentesByEstadoAsync(EstadoIncidente estado);
    Task<ApiResponse<bool>> AsignarIncidenteAsync(int incidenteId, int tecnicoId, int asignadoPorId);
    Task<ApiResponse<bool>> CerrarIncidenteAsync(int incidenteId, string solucion, int cerradoPorId);
    Task<ApiResponse<Dictionary<EstadoIncidente, int>>> GetEstadisticasEstadoAsync();
    Task<ApiResponse<Dictionary<PrioridadIncidente, int>>> GetEstadisticasPrioridadAsync();
}