using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.Rol)
                .Where(u => u.Activo)
                .ToListAsync();
        }

        public override async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            return await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);
        }

        /// <summary>
        /// Obtiene técnicos del nivel de soporte especificado (para asignación)
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetTecnicosPorNivelAsync(int idNivelSoporte)
        {
            // Asumiendo que los roles tienen nombres específicos: "Técnico N1", "Técnico N2", "Técnico N3"
            var nombreNivel = $"Técnico N{idNivelSoporte}";
            
            return await _dbSet
                .Include(u => u.Rol)
                .Where(u => u.Activo && u.Rol.Nombre.Contains(nombreNivel))
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un usuario es SPOC (Single Point of Contact)
        /// </summary>
        public async Task<bool> EsSPOCAsync(int idUsuario)
        {
            var usuario = await _dbSet
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            return usuario?.Rol.Nombre == "SPOC";
        }
    }
}