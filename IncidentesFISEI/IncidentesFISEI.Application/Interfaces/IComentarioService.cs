using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces;

public interface IComentarioService
{
    Task<IEnumerable<ComentarioDto>> GetComentariosByIncidenteIdAsync(int incidenteId);
    Task<ComentarioDto> GetComentarioByIdAsync(int id);
    Task<ComentarioDto> CreateComentarioAsync(CreateComentarioDto createComentarioDto);
    Task<ComentarioDto> UpdateComentarioAsync(int id, UpdateComentarioDto updateComentarioDto);
    Task<bool> DeleteComentarioAsync(int id);
}