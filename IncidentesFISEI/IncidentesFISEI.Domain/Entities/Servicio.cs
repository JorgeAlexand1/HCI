namespace IncidentesFISEI.Domain.Entities;

public class Servicio : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; } // Código único del servicio (ej: "SRV-001")
    public bool IsActive { get; set; } = true;
    
    // Relación con Categoría
    public int CategoriaId { get; set; }
    public CategoriaIncidente Categoria { get; set; } = null!;
    
    // Información del servicio
    public string? ResponsableArea { get; set; } // Área o departamento responsable
    public string? ContactoTecnico { get; set; } // Email o contacto del responsable técnico
    public int? TiempoRespuestaMinutos { get; set; } // SLA específico del servicio
    public int? TiempoResolucionMinutos { get; set; } // SLA específico del servicio
    
    // Configuración adicional
    public string? Instrucciones { get; set; } // Instrucciones específicas para este servicio
    public string? EscalacionProcedure { get; set; } // Procedimiento de escalación
    public bool RequiereAprobacion { get; set; } = false; // Si requiere aprobación adicional
    
    // Navegación
    public ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}