using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Representa una versión histórica de un artículo de conocimiento (ITIL Knowledge Management)
/// </summary>
public class VersionArticulo : BaseEntity
{
    public int ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento ArticuloConocimiento { get; set; } = null!;
    
    public int NumeroVersion { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? Resumen { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    
    public string? CambiosRealizados { get; set; } // Changelog
    public int ModificadoPorId { get; set; }
    public Usuario ModificadoPor { get; set; } = null!;
    
    public DateTime FechaVersion { get; set; } = DateTime.UtcNow;
}
