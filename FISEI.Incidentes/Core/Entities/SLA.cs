using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("SLA")]
    public class SLA
    {
        [Key]
        public int IdSLA { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        // Servicio al que aplica el SLA
        [Required]
        public int IdServicio { get; set; }
    }
}
