using IncidentesFISEI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

public class ArticuloConocimientoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public TipoArticulo Tipo { get; set; }
    public int AutorId { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
    public bool EsPublico { get; set; }
    public string Tags { get; set; } = string.Empty;
    public int VotosPositivos { get; set; }
    public int VotosNegativos { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

public class CreateArticuloConocimientoDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    public string Contenido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de artículo es requerido")]
    public TipoArticulo Tipo { get; set; }

    public int AutorId { get; set; }

    public bool EsPublico { get; set; } = false;

    [MaxLength(500, ErrorMessage = "Los tags no pueden exceder 500 caracteres")]
    public string Tags { get; set; } = string.Empty;
}

public class UpdateArticuloConocimientoDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    public string Contenido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de artículo es requerido")]
    public TipoArticulo Tipo { get; set; }

    public bool EsPublico { get; set; } = false;

    [MaxLength(500, ErrorMessage = "Los tags no pueden exceder 500 caracteres")]
    public string Tags { get; set; } = string.Empty;
}