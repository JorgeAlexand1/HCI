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

        [ForeignKey("Rol")]
        public int IdRol { get; set; }
        public Rol Rol { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}