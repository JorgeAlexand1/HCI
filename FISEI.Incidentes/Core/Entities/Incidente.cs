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

        // ITIL v3: atributos clave para priorización y seguimiento
        [StringLength(20)]
        public string? Prioridad { get; set; } // P1..P5

        [StringLength(20)]
        public string? Impacto { get; set; } // Alto/Medio/Bajo

        [StringLength(20)]
        public string? Urgencia { get; set; } // Alta/Media/Baja

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }

        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaPrimeraRespuesta { get; set; }

        // ITIL v3: resolución y causa raíz
        [StringLength(1000)]
        public string? Resolucion { get; set; }

        [StringLength(1000)]
        public string? CausaRaiz { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // Propietario actual (técnico asignado) para trazabilidad
        public int? IdUsuarioPropietarioActual { get; set; }

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