using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repositorio especializado para gestión de incidentes según ITIL v3
    /// </summary>
    public class IncidenteRepository : Repository<Incidente>, IIncidenteRepository
    {
        public IncidenteRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene incidentes con todas sus relaciones (Eager Loading)
        /// </summary>
        public override async Task<IEnumerable<Incidente>> GetAllAsync()
        {
            return await _dbSet
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Include(i => i.NivelSoporte)
                .Include(i => i.Servicio)
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
        }

        public override async Task<Incidente?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Include(i => i.NivelSoporte)
                .Include(i => i.Servicio)
                .FirstOrDefaultAsync(i => i.IdIncidente == id);
        }

        public async Task<IEnumerable<Incidente>> GetIncidentesPorEstadoAsync(int idEstado)
        {
            return await _dbSet
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Include(i => i.NivelSoporte)
                .Where(i => i.IdEstado == idEstado)
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidente>> GetIncidentesPorUsuarioAsync(int idUsuario)
        {
            return await _dbSet
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Include(i => i.NivelSoporte)
                .Where(i => i.IdUsuario == idUsuario)
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidente>> GetIncidentesPorNivelAsync(int idNivelSoporte)
        {
            return await _dbSet
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Where(i => i.IdNivelSoporte == idNivelSoporte)
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene incidentes sin asignar (para SPOC)
        /// </summary>
        public async Task<IEnumerable<Incidente>> GetIncidentesSinAsignarAsync()
        {
            var incidentesConAsignacionActiva = await _context.Asignaciones
                .Where(a => a.Activo)
                .Select(a => a.IdIncidente)
                .ToListAsync();

            return await _dbSet
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .Include(i => i.NivelSoporte)
                .Where(i => !incidentesConAsignacionActiva.Contains(i.IdIncidente))
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
        }

        /// <summary>
        /// Detecta incidentes recurrentes para escalamiento automático (ITIL Problem Management)
        /// </summary>
        public async Task<IEnumerable<Incidente>> GetIncidentesRecurrentesAsync(int idCategoria, int minOcurrencias)
        {
            return await _dbSet
                .Where(i => i.IdCategoria == idCategoria)
                .GroupBy(i => new { i.IdCategoria, i.Titulo })
                .Where(g => g.Count() >= minOcurrencias)
                .SelectMany(g => g)
                .Include(i => i.Usuario)
                .Include(i => i.Categoria)
                .Include(i => i.Estado)
                .ToListAsync();
        }

        /// <summary>
        /// Cuenta incidentes asignados a un técnico (para distribución equitativa)
        /// </summary>
        public async Task<int> ContarIncidentesPorTecnicoAsync(int idTecnico)
        {
            return await _context.Asignaciones
                .Where(a => a.IdUsuarioAsignado == idTecnico && a.Activo)
                .CountAsync();
        }
    }
}