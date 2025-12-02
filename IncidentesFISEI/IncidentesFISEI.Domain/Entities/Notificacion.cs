using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class Notificacion : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int? IncidenteId { get; set; }
    public Incidente? Incidente { get; set; }

    public TipoNotificacion Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    
    public bool Leida { get; set; } = false;
    public DateTime? FechaLectura { get; set; }
    
    public bool EnviadaPorEmail { get; set; } = false;
    public DateTime? FechaEnvioEmail { get; set; }
    
    // Metadata adicional (JSON serializado)
    public string? MetadataJson { get; set; }
    
    // Para agrupar notificaciones relacionadas
    public string? GrupoNotificacion { get; set; }
}
