using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("NIVEL_SOPORTE")]
    public class NivelSoporte
    {
        [Key]
        public int IdNivelSoporte { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }
}
