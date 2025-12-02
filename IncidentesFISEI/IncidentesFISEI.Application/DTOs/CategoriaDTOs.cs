using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

public class CategoriaIncidenteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CreateCategoriaIncidenteDto
{
    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
}

public class UpdateCategoriaIncidenteDto
{
    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}