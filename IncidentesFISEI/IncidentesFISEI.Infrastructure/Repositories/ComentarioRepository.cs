using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentesFISEI.Infrastructure.Repositories;

public class ComentarioRepository : Repository<ComentarioIncidente>, IComentarioRepository
{
    public ComentarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ComentarioIncidente>> GetComentariosByIncidenteIdAsync(int incidenteId)
    {
        return await _context.ComentariosIncidente
            .Include(c => c.Autor)
            .Where(c => c.IncidenteId == incidenteId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ComentarioIncidente>> GetComentariosByUsuarioIdAsync(int usuarioId)
    {
        return await _context.ComentariosIncidente
            .Include(c => c.Incidente)
            .Where(c => c.AutorId == usuarioId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}