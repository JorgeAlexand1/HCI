using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FISEI.Incidentes.Core.Identity;
using FISEI.Incidentes.Infrastructure.Data;
using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IdentitySeeder> _logger;

        private readonly string[] _roles = new[]
        {
            "Admin","ServiceDesk","SupportN1","SupportN2","SupportN3","Student","Professor","Staff","KnowledgeManager","ProblemManager","ChangeManager"
        };

        public IdentitySeeder(RoleManager<IdentityRole> roleManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration configuration,
                              ApplicationDbContext db,
                              ILogger<IdentitySeeder> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await EnsureRolesAsync();
            await EnsureAdminUserAsync();
        }

        private async Task EnsureRolesAsync()
        {
            foreach (var role in _roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        _logger.LogWarning("No se pudo crear el rol {Role}: {Errors}", role, string.Join(",", result.Errors.Select(e => e.Description)));
                }
            }
        }

        private async Task EnsureAdminUserAsync()
        {
            var adminConfig = _configuration.GetSection("AdminSeed");
            var email = adminConfig.GetValue<string>("Email");
            var password = adminConfig.GetValue<string>("Password");
            var displayName = adminConfig.GetValue<string>("DisplayName");
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("AdminSeed configuración incompleta, saltando creación de usuario admin.");
                return;
            }

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                _logger.LogInformation("Usuario admin ya existe.");
                // Asegurar vinculación con tabla de dominio aunque el usuario ya exista
                var linked = _db.Usuarios.FirstOrDefault(u => u.IdentityUserId == existing.Id);
                if (linked == null)
                {
                    _db.Usuarios.Add(new Usuario
                    {
                        Nombre = displayName ?? email,
                        Correo = email,
                        Contrasena = "(identity-managed)",
                        IdentityUserId = existing.Id,
                        Activo = true
                    });
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Se añadió registro en Usuarios para el admin existente.");
                }
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                NombreMostrar = displayName,
                EmailConfirmed = true,
                Activo = true
            };
            var create = await _userManager.CreateAsync(user, password);
            if (!create.Succeeded)
            {
                _logger.LogError("Error creando usuario admin: {Errors}", string.Join(",", create.Errors.Select(e => e.Description)));
                return;
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            // Vincular con dominio Usuario
            _db.Usuarios.Add(new Usuario
            {
                Nombre = displayName ?? email,
                Correo = email,
                Contrasena = "(identity-managed)",
                IdentityUserId = user.Id,
                Activo = true
            });
            await _db.SaveChangesAsync();
            _logger.LogInformation("Usuario admin creado y vinculado exitosamente.");
        }
    }
}
