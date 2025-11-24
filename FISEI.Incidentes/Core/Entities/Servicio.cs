using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("SERVICIO")]
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        [StringLength(100)]
        public string? Responsable { get; set; }

        [StringLength(100)]
        public string? SLA { get; set; }

        [StringLength(100)]
        public string? AreaDestino { get; set; }
    }
}
