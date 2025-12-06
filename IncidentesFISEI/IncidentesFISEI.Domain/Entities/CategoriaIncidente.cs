namespace IncidentesFISEI.Domain.Entities;

public class CategoriaIncidente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Color { get; set; } // Para la UI
    public string? Icono { get; set; } // Para la UI
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }
    
    // Configuración de SLA por categoría
    public int? TiempoRespuestaMinutos { get; set; }
    public int? TiempoResolucionMinutos { get; set; }
    
    // Navegación
    public CategoriaIncidente? ParentCategory { get; set; }
    public ICollection<CategoriaIncidente> SubCategorias { get; set; } = new List<CategoriaIncidente>();
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    public ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
    public ICollection<ArticuloConocimiento> ArticulosConocimiento { get; set; } = new List<ArticuloConocimiento>();
}