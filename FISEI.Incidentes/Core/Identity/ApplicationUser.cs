using Microsoft.AspNetCore.Identity;

namespace FISEI.Incidentes.Core.Identity
{
    // Identity user con clave string (por defecto). Si se desea int, habría que reconfigurar todo.
    public class ApplicationUser : IdentityUser
    {
        // Campos adicionales opcionales para perfil
        public bool Activo { get; set; } = true;
        public string? NombreMostrar { get; set; }
    }
}
