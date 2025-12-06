using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Infrastructure.Data;

namespace IncidentesFISEI.Infrastructure.Repositories;

public class ServicioRepository : Repository<Servicio>, IServicioRepository
{
    public ServicioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Servicio>> GetByCategoriaIdAsync(int categoriaId)
    {
        return await _context.Servicios
            .Include(s => s.Categoria)
            .Where(s => s.CategoriaId == categoriaId && s.IsActive)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Servicio>> GetActiveServiciosAsync()
    {
        return await _context.Servicios
            .Include(s => s.Categoria)
            .Where(s => s.IsActive && s.Categoria.IsActive)
            .OrderBy(s => s.Categoria.Nombre)
            .ThenBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<Servicio?> GetByCodigoAsync(string codigo)
    {
        return await _context.Servicios
            .Include(s => s.Categoria)
            .FirstOrDefaultAsync(s => s.Codigo == codigo);
    }

    public async Task<IEnumerable<Servicio>> SearchServiciosAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await _context.Servicios
            .Include(s => s.Categoria)
            .Where(s => s.IsActive && 
                       (s.Nombre.ToLower().Contains(normalizedSearchTerm) ||
                        s.Descripcion.ToLower().Contains(normalizedSearchTerm) ||
                        s.Codigo!.ToLower().Contains(normalizedSearchTerm) ||
                        s.Categoria.Nombre.ToLower().Contains(normalizedSearchTerm)))
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _context.Servicios.Where(s => s.Codigo == codigo);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public new async Task<Servicio?> GetByIdAsync(int id)
    {
        return await _context.Servicios
            .Include(s => s.Categoria)
            .Include(s => s.Incidentes)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public new async Task<IEnumerable<Servicio>> GetAllAsync()
    {
        return await _context.Servicios
            .Include(s => s.Categoria)
            .OrderBy(s => s.Categoria.Nombre)
            .ThenBy(s => s.Nombre)
            .ToListAsync();
    }
}