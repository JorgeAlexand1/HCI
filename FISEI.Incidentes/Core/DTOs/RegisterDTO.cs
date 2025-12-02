using System.ComponentModel.DataAnnotations;

namespace FISEI.Incidentes.Core.DTOs
{
    /// <summary>
    /// DTO para el registro de nuevos usuarios
    /// </summary>
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para asignar rol a un usuario (solo Admin)
    /// </summary>
    public class AsignarRolDTO
    {
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El ID del rol es requerido")]
        public int IdRol { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de usuario con rol
    /// </summary>
    public class UsuarioConRolDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool EmailVerificado { get; set; }
        public int? IdRol { get; set; }
        public string? NombreRol { get; set; }
        public string? DescripcionRol { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de rol
    /// </summary>
    public class RolDTO
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
