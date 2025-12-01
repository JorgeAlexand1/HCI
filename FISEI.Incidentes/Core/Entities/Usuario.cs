using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, StringLength(100)]
        public string Correo { get; set; } = null!;

        [Required, StringLength(100)]
        public string Contrasena { get; set; } = null!;

        public bool Activo { get; set; } = true;

        // Relación con Identity (AspNetUsers) para autenticación
        [StringLength(450)]
        public string? IdentityUserId { get; set; }

        // Rol de dominio (opcional)
        [ForeignKey("Rol")]
        public int? IdRol { get; set; }
        public Rol? Rol { get; set; }

        // Recuperación de contraseña
        [StringLength(200)]
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // Verificación de correo
        public bool EmailVerificado { get; set; } = false;
        [StringLength(200)]
        public string? EmailVerificationToken { get; set; }
    }
}