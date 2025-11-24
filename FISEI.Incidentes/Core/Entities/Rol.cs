using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("ROL")]
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; } = null!;

        [StringLength(150)]
        public string? Descripcion { get; set; }
    }
}
