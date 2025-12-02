using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.DTOs;

public class NotificacionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    
    public int? IncidenteId { get; set; }
    public string? IncidenteTitulo { get; set; }
    
    public TipoNotificacion Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    
    public bool Leida { get; set; }
    public DateTime? FechaLectura { get; set; }
    
    public bool EnviadaPorEmail { get; set; }
    public DateTime? FechaEnvioEmail { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string TiempoTranscurrido { get; set; } = string.Empty;
}

public class CreateNotificacionDto
{
    public int UsuarioId { get; set; }
    public int? IncidenteId { get; set; }
    public TipoNotificacion Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool EnviarEmail { get; set; } = true;
    public string? MetadataJson { get; set; }
}

public class NotificacionesPaginadasDto
{
    public List<NotificacionDto> Notificaciones { get; set; } = new();
    public int TotalNotificaciones { get; set; }
    public int NoLeidas { get; set; }
    public int Pagina { get; set; }
    public int TamañoPagina { get; set; }
    public int TotalPaginas { get; set; }
}

public class EstadisticasNotificacionesDto
{
    public int TotalNotificaciones { get; set; }
    public int NoLeidas { get; set; }
    public int Leidas { get; set; }
    public int EnviadasPorEmail { get; set; }
    public Dictionary<string, int> NotificacionesPorTipo { get; set; } = new();
    public List<NotificacionDto> UltimasNotificaciones { get; set; } = new();
}
