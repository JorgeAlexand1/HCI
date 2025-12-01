using System;
using System.ComponentModel.DataAnnotations;

namespace FISEI.Incidentes.Core.DTOs
{
    /// <summary>
    /// DTO para creaci�n de incidentes
    /// </summary>
    public class CrearIncidenteDTO
    {
        [Required(ErrorMessage = "El t�tulo es obligatorio")]
        [StringLength(200, ErrorMessage = "El t�tulo no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "La descripci�n es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripci�n no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una categor�a")]
        public int IdCategoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un servicio")]
        public int IdServicio { get; set; }

        public int IdUsuario { get; set; }

        // ITIL v3: opcionales para priorización
        [StringLength(20)]
        public string? Prioridad { get; set; }

        [StringLength(20)]
        public string? Impacto { get; set; }

        [StringLength(20)]
        public string? Urgencia { get; set; }
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