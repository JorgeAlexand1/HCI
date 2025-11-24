using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("HISTORIAL")]
    public class Historial
    {
        [Key]
        public int IdHistorial { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required, StringLength(500)]
        public string Comentario { get; set; } = null!;

        [ForeignKey("Incidente")]
        public int IdIncidente { get; set; }
        public Incidente Incidente { get; set; } = null!;

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}   