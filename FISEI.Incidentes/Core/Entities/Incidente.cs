using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    [Table("INCIDENTE")]
    public class Incidente
    {
        [Key]
        public int IdIncidente { get; set; }

        [Required, StringLength(200)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Descripcion { get; set; } = null!;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }
        public Estado Estado { get; set; } = null!;

        [ForeignKey("NivelSoporte")]
        public int IdNivelSoporte { get; set; }
        public NivelSoporte NivelSoporte { get; set; } = null!;

        [ForeignKey("Servicio")]
        public int IdServicio { get; set; }
        public Servicio Servicio { get; set; } = null!;
    }
}