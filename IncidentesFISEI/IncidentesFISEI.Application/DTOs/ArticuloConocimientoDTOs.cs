using IncidentesFISEI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

public class ArticuloConocimientoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? Resumen { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public EstadoArticulo Estado { get; set; }
    public int Visualizaciones { get; set; }
    public int VotosPositivos { get; set; }
    public int VotosNegativos { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaRevision { get; set; }
    public bool EsSolucionValidada { get; set; }
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    
    public int AutorId { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
    public int? RevisadoPorId { get; set; }
    public string? RevisadoPorNombre { get; set; }
    
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateArticuloConocimientoDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    public string Contenido { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Resumen { get; set; }

    public int AutorId { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    public string Tags { get; set; } = string.Empty;
    
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }

    public int? IncidenteRelacionadoId { get; set; }
}

public class UpdateArticuloConocimientoDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    public string Contenido { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Resumen { get; set; }

    public string Tags { get; set; } = string.Empty;
    
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }

    public int CategoriaId { get; set; }
}