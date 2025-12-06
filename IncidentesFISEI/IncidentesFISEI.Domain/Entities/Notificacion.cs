using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Entidad para el sistema de notificaciones ITIL v3
/// Maneja comunicaciones automáticas según eventos del sistema
/// </summary>
public class Notificacion : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public TipoNotificacion TipoNotificacion { get; set; }
    public PrioridadNotificacion Prioridad { get; set; } = PrioridadNotificacion.Normal;
    public bool Leida { get; set; } = false;
    public DateTime? FechaLectura { get; set; }
    
    // Datos adicionales en formato JSON para flexibilidad
    public string? DatosAdicionales { get; set; }
    
    // URL de acción (ej: /incidentes/123)
    public string? UrlAccion { get; set; }
    
    // Relaciones
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public int? IncidenteId { get; set; }
    public Incidente? Incidente { get; set; }
    
    // Datos de entrega
    public bool NotificadoPorEmail { get; set; } = false;
    public bool NotificadoPorSMS { get; set; } = false;
    public DateTime? FechaEnvioEmail { get; set; }
    public DateTime? FechaEnvioSMS { get; set; }
    public string? ErrorEnvio { get; set; }
    
    // Navegación para logs de envío
    public ICollection<LogNotificacion> LogsEnvio { get; set; } = new List<LogNotificacion>();
    
    /// <summary>
    /// Marca la notificación como leída
    /// </summary>
    public void MarcarComoLeida()
    {
        Leida = true;
        FechaLectura = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Configuración de preferencias de notificaciones por usuario
/// </summary>
public class ConfiguracionNotificacion : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public TipoEventoNotificacion TipoEvento { get; set; }
    
    // Canales de notificación
    public bool NotificarEnSistema { get; set; } = true;
    public bool NotificarPorEmail { get; set; } = true;
    public bool NotificarPorSMS { get; set; } = false;
    
    // Configuración de frecuencia
    public bool NotificacionInmediata { get; set; } = true;
    public bool ResumenDiario { get; set; } = false;
    public bool ResumenSemanal { get; set; } = false;
    
    // Filtros
    public PrioridadIncidente? SoloPrioridad { get; set; }
    public bool SoloIncidentesAsignados { get; set; } = false;
    
    // Horarios de silencio (modo no molestar)
    public TimeOnly? HoraInicioSilencioso { get; set; }
    public TimeOnly? HoraFinSilencioso { get; set; }
}

/// <summary>
/// Plantillas para diferentes tipos de notificaciones
/// Siguiendo estándares ITIL v3 de comunicación
/// </summary>
public class PlantillaNotificacion : BaseEntity
{
    public TipoNotificacion TipoNotificacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string PlantillaTitulo { get; set; } = string.Empty;
    public string PlantillaMensaje { get; set; } = string.Empty;
    public string? PlantillaEmail { get; set; }
    public string? PlantillaSMS { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Variables disponibles (ej: {UsuarioNombre}, {NumeroIncidente}, etc.)
    public string? VariablesDisponibles { get; set; }
}

/// <summary>
/// Log de notificaciones enviadas para auditoría
/// Cumple con requisitos de trazabilidad ITIL v3
/// </summary>
public class LogNotificacion : BaseEntity
{
    public int NotificacionId { get; set; }
    public Notificacion Notificacion { get; set; } = null!;
    
    public CanalNotificacion Canal { get; set; }
    public EstadoEnvioNotificacion Estado { get; set; }
    
    public DateTime FechaIntento { get; set; }
    public DateTime? FechaEntrega { get; set; }
    
    public string? DireccionDestino { get; set; } // Email, teléfono, etc.
    public string? MensajeError { get; set; }
    public string? ErrorDetalle { get; set; } // Detalles técnicos del error
    public string? IdExterno { get; set; } // ID del proveedor de email/SMS
    
    public int NumeroReintentos { get; set; } = 0;
    public DateTime? ProximoReintento { get; set; }
}