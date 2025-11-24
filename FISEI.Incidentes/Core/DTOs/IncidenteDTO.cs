using System;
using System.ComponentModel.DataAnnotations;

namespace FISEI.Incidentes.Core.DTOs
{
    /// <summary>
    /// DTO para creación de incidentes
    /// </summary>
    public class CrearIncidenteDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una categoría")]
        public int IdCategoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un servicio")]
        public int IdServicio { get; set; }

        public int IdUsuario { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de incidente con datos completos
    /// </summary>
    public class IncidenteDetalleDTO
    {
        public int IdIncidente { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        
        // Datos relacionados
        public string NombreUsuario { get; set; } = null!;
        public string NombreCategoria { get; set; } = null!;
        public string NombreEstado { get; set; } = null!;
        public string NombreNivelSoporte { get; set; } = null!;
        public string NombreServicio { get; set; } = null!;
        public string? TecnicoAsignado { get; set; }
    }
}