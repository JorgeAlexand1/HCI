using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("ASIGNACION")]
    public class Asignacion
    {
        [Key]
        public int IdAsignacion { get; set; }

        [ForeignKey("Incidente")]
        public int IdIncidente { get; set; }
        public Incidente Incidente { get; set; } = null!;

        [ForeignKey("Usuario")]
        public int IdUsuarioAsignado { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}