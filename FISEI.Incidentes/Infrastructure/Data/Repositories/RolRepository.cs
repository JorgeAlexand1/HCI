using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Infrastructure.Data.Repositories
{
    public class RolRepository : Repository<Rol>, IRolRepository
    {
        public RolRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Rol?> GetByNombreAsync(string nombre)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Nombre == nombre);
        }

        /// <summary>
        /// Obtiene roles disponibles para asignación por el administrador
        /// </summary>
        public async Task<IEnumerable<Rol>> GetRolesDisponiblesParaAsignacionAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
