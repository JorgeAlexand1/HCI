using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Etiqueta reutilizable para clasificación de artículos
/// </summary>
public class EtiquetaConocimiento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Color { get; set; } = "#007bff"; // Color hexadecimal para UI
    public int VecesUsada { get; set; } = 0;
    
    public ICollection<ArticuloEtiqueta> ArticulosEtiquetas { get; set; } = new List<ArticuloEtiqueta>();
}

/// <summary>
/// Tabla intermedia para relación muchos a muchos
/// </summary>
public class ArticuloEtiqueta : BaseEntity
{
    public int ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento ArticuloConocimiento { get; set; } = null!;
    
    public int EtiquetaId { get; set; }
    public EtiquetaConocimiento Etiqueta { get; set; } = null!;
}
