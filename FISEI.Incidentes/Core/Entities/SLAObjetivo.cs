using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("SLA_OBJETIVO")]
    public class SLAObjetivo
    {
        [Key]
        public int IdSLAObjetivo { get; set; }

        [Required]
        public int IdSLA { get; set; }

        // Por prioridad: P1..P5
        [Required, StringLength(20)]
        public string Prioridad { get; set; } = null!;

        // Minutos comprometidos de primera respuesta y resolución
        public int MinutosPrimeraRespuesta { get; set; }
        public int MinutosResolucion { get; set; }
    }
}
