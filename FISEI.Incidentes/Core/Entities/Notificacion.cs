using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("NOTIFICACION")]
    public class Notificacion
    {
        [Key]
        public int IdNotificacion { get; set; }

        [Required, StringLength(200)]
        public string Mensaje { get; set; } = null!;

        public bool Leido { get; set; } = false;

        [Required]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [ForeignKey("Usuario")]
        public int IdUsuarioDestino { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }

}