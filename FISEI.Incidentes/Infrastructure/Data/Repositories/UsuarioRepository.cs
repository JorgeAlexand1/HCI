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
                .Where(u => u.Activo)
                .ToListAsync();
        }

        public override async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);
        }

        /// <summary>
        /// Obtiene t�cnicos del nivel de soporte especificado (para asignaci�n)
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetTecnicosPorNivelAsync(int idNivelSoporte)
        {
            // Filtrar por roles de dominio tipo SupportN{nivel}
            var roleName = $"SupportN{idNivelSoporte}";
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Activo && u.Rol != null && u.Rol.Nombre == roleName)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un usuario es SPOC (Single Point of Contact)
        /// </summary>
        public async Task<bool> EsSPOCAsync(int idUsuario)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (usuario?.Rol?.Nombre == null) return false;
            return usuario.Rol.Nombre == "ServiceDesk" || usuario.Rol.Nombre == "SPOC";
        }
    }
}