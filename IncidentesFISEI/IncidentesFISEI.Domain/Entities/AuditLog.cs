using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Registro de auditoría para trazabilidad completa de operaciones
/// </summary>
public class AuditLog : BaseEntity
{
    // Información del usuario
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string? UsuarioNombre { get; set; } // Snapshot del nombre en caso de que el usuario sea eliminado
    public string? DireccionIP { get; set; }
    public string? UserAgent { get; set; }
    
    // Información de la acción
    public TipoAccionAuditoria TipoAccion { get; set; }
    public TipoEntidadAuditoria TipoEntidad { get; set; }
    public int? EntidadId { get; set; } // ID de la entidad afectada
    public string? EntidadDescripcion { get; set; } // Descripción de la entidad (ej: "Incidente #INC-2024-001")
    
    // Detalles de la acción
    public string Descripcion { get; set; } = string.Empty;
    public string? ValoresAnteriores { get; set; } // JSON con valores antes del cambio
    public string? ValoresNuevos { get; set; } // JSON con valores después del cambio
    
    // Metadata adicional
    public NivelSeveridadAuditoria NivelSeveridad { get; set; } = NivelSeveridadAuditoria.Informativo;
    public string? MetadataJson { get; set; } // JSON con información adicional
    public bool EsExitoso { get; set; } = true;
    public string? MensajeError { get; set; }
    
    // Para auditoría de consultas masivas
    public int? CantidadRegistros { get; set; }
    public string? FiltrosAplicados { get; set; }
    
    // Trazabilidad
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public string? Modulo { get; set; } // Ej: "API", "BackgroundService", "Sistema"
    public string? Endpoint { get; set; } // URL del endpoint si viene de API
}
