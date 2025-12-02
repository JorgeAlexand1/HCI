using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class IncidenteRelacionado : BaseEntity
{
    public int IncidentePrincipalId { get; set; }
    public Incidente IncidentePrincipal { get; set; } = null!;
    
    public int IncidenteRelacionadoId { get; set; }
    public Incidente IncidenteRef { get; set; } = null!;
    
    public TipoRelacion TipoRelacion { get; set; }
    public string? Descripcion { get; set; }
}

public class RegistroTiempo : BaseEntity
{
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public TimeSpan TiempoTranscurrido { get; set; }
    public string? Descripcion { get; set; }
    public TipoActividad TipoActividad { get; set; }
    
    // Relaciones
    public int IncidenteId { get; set; }
    public Incidente Incidente { get; set; } = null!;
    
    public int TecnicoId { get; set; }
    public Usuario Tecnico { get; set; } = null!;
}

public class SLA : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    
    // Tiempos en minutos
    public int TiempoRespuesta { get; set; }
    public int TiempoResolucion { get; set; }
    
    // Configuración por prioridad
    public PrioridadIncidente Prioridad { get; set; }
    public ImpactoIncidente Impacto { get; set; }
    public UrgenciaIncidente Urgencia { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class EscalacionSLA : BaseEntity
{
    public int SLAId { get; set; }
    public SLA SLA { get; set; } = null!;
    
    public int IncidenteId { get; set; }
    public Incidente Incidente { get; set; } = null!;
    
    public DateTime FechaEscalacion { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public bool FueNotificado { get; set; } = false;
}