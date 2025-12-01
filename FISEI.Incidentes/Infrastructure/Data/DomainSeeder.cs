using FISEI.Incidentes.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FISEI.Incidentes.Infrastructure.Data
{
    public class DomainSeeder
    {
        private readonly ApplicationDbContext _db;
        public DomainSeeder(ApplicationDbContext db) { _db = db; }

        public async Task SeedAsync()
        {
            // Roles básicos
            var roles = new[] { "SPOC", "ServiceDesk", "SupportN1", "SupportN2", "SupportN3" };
            foreach (var r in roles)
            {
                if (!await _db.Set<Rol>().AnyAsync(x => x.Nombre == r))
                    _db.Set<Rol>().Add(new Rol { Nombre = r, Descripcion = $"Rol {r}" });
            }
            await _db.SaveChangesAsync();

            // Estados básicos de incidente
            var estados = new[] { "Nuevo", "Asignado", "En Progreso", "Resuelto", "Cerrado" };
            foreach (var e in estados)
            {
                if (!await _db.EstadosIncidentes.AnyAsync(x => x.Nombre == e))
                    _db.EstadosIncidentes.Add(new Estado { Nombre = e });
            }
            await _db.SaveChangesAsync();

            // Categorías base
            var categorias = new[] { "Hardware", "Software", "Red", "Accesos", "Correo" };
            foreach (var c in categorias)
            {
                if (!await _db.Categorias.AnyAsync(x => x.Nombre == c))
                    _db.Categorias.Add(new Categoria { Nombre = c });
            }
            await _db.SaveChangesAsync();

            // Servicios base
            var servicios = new[] { "Impresión", "Correo Electrónico", "VPN", "Internet", "ERP" };
            foreach (var s in servicios)
            {
                if (!await _db.Servicios.AnyAsync(x => x.Nombre == s))
                    _db.Servicios.Add(new Servicio { Nombre = s });
            }
            await _db.SaveChangesAsync();

            // Niveles de soporte
            var niveles = new[] { "Nivel 1", "Nivel 2", "Nivel 3" };
            foreach (var nombre in niveles)
            {
                if (!await _db.NivelesSoporte.AnyAsync(x => x.Nombre == nombre))
                    _db.NivelesSoporte.Add(new NivelSoporte { Nombre = nombre });
            }
            await _db.SaveChangesAsync();

            // Usuario demo si no existe
            if (!await _db.Usuarios.AnyAsync(u => u.Correo == "demo@fisei.local"))
            {
                var rolN1 = await _db.Set<Rol>().FirstAsync(r => r.Nombre == "SupportN1");
                _db.Usuarios.Add(new Usuario
                {
                    Nombre = "Usuario Demo",
                    Correo = "demo@fisei.local",
                    Contrasena = Infrastructure.Security.PasswordHasher.HashPassword("Demo#2025"),
                    Activo = true,
                    IdRol = rolN1.IdRol
                });
                await _db.SaveChangesAsync();
            }
        }
    }
}
