using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Domain.Interfaces;

public interface IComentarioRepository : IRepository<ComentarioIncidente>
{
    Task<IEnumerable<ComentarioIncidente>> GetComentariosByIncidenteIdAsync(int incidenteId);
    Task<IEnumerable<ComentarioIncidente>> GetComentariosByUsuarioIdAsync(int usuarioId);
}