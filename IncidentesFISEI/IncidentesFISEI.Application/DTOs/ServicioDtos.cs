namespace IncidentesFISEI.Application.DTOs;

public class ServicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public bool IsActive { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public string? ResponsableArea { get; set; }
    public string? ContactoTecnico { get; set; }
    public int? TiempoRespuestaMinutos { get; set; }
    public int? TiempoResolucionMinutos { get; set; }
    public string? Instrucciones { get; set; }
    public string? EscalacionProcedure { get; set; }
    public bool RequiereAprobacion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ServicioListDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public bool IsActive { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string? ResponsableArea { get; set; }
    public int IncidentesActivos { get; set; }
    public int TotalIncidentes { get; set; }
}

public class CreateServicioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public int CategoriaId { get; set; }
    public string? ResponsableArea { get; set; }
    public string? ContactoTecnico { get; set; }
    public int? TiempoRespuestaMinutos { get; set; }
    public int? TiempoResolucionMinutos { get; set; }
    public string? Instrucciones { get; set; }
    public string? EscalacionProcedure { get; set; }
    public bool RequiereAprobacion { get; set; } = false;
}

public class UpdateServicioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public int CategoriaId { get; set; }
    public string? ResponsableArea { get; set; }
    public string? ContactoTecnico { get; set; }
    public int? TiempoRespuestaMinutos { get; set; }
    public int? TiempoResolucionMinutos { get; set; }
    public string? Instrucciones { get; set; }
    public string? EscalacionProcedure { get; set; }
    public bool RequiereAprobacion { get; set; }
    public bool IsActive { get; set; }
}