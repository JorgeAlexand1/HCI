using FISEI.Incidentes.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FISEI.Incidentes.Core.Entities
{
    /// <summary>
    /// Base de Conocimiento - Almacena soluciones de incidentes resueltos
    /// </summary>
    [Table("CONOCIMIENTO")]
    public class Conocimiento
    {
        [Key]
        public int IdConocimiento { get; set; }

        [Required, StringLength(250)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Descripcion { get; set; } = null!;

        [Required]
        public string Solucion { get; set; } = null!;

        [StringLength(500)]
        public string? PalabrasClave { get; set; }

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey("Incidente")]
        public int? IdIncidenteOrigen { get; set; }
        public Incidente? IncidenteOrigen { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int Visualizaciones { get; set; } = 0;

        public int? Calificacion { get; set; } // 1-5 estrellas

        public bool Aprobado { get; set; } = false;
    }
}