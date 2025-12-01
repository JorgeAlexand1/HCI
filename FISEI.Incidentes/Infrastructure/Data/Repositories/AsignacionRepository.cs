using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Infrastructure.Data.Repositories
{
    public class AsignacionRepository : Repository<Asignacion>, IAsignacionRepository
    {
        public AsignacionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Asignacion>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Incidente)
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaAsignacion)
                .ToListAsync();
        }

        public async Task<Asignacion?> GetAsignacionActivaPorIncidenteAsync(int idIncidente)
        {
            return await _dbSet
                .Include(a => a.Usuario)
                .Include(a => a.Incidente)
                .FirstOrDefaultAsync(a => a.IdIncidente == idIncidente && a.Activo);
        }

        public async Task<IEnumerable<Asignacion>> GetAsignacionesPorTecnicoAsync(int idTecnico)
        {
            return await _dbSet
                .Include(a => a.Incidente)
                    .ThenInclude(i => i.Estado)
                .Where(a => a.IdUsuarioAsignado == idTecnico && a.Activo)
                .OrderByDescending(a => a.FechaAsignacion)
                .ToListAsync();
        }

        /// <summary>
        /// Desactiva asignaciones anteriores al reasignar un incidente
        /// </summary>
        public async Task DesactivarAsignacionesAnterioresAsync(int idIncidente)
        {
            var asignacionesAnteriores = await _dbSet
                .Where(a => a.IdIncidente == idIncidente && a.Activo)
                .ToListAsync();

            foreach (var asignacion in asignacionesAnteriores)
            {
                asignacion.Activo = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}