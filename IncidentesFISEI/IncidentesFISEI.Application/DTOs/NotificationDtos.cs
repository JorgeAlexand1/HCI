namespace IncidentesFISEI.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string TipoNotificacion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public int? IncidenteId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLectura { get; set; }
}

public class NotificationSettingsDto
{
    public string TipoEvento { get; set; } = string.Empty;
    public bool NotificarEnSistema { get; set; }
    public bool NotificarPorEmail { get; set; }
    public bool NotificarPorSMS { get; set; }
    public bool NotificacionInmediata { get; set; }
    public TimeOnly? HoraInicioSilencioso { get; set; }
    public TimeOnly? HoraFinSilencioso { get; set; }
}

public class UpdateNotificationSettingsDto
{
    public bool NotificarEnSistema { get; set; }
    public bool NotificarPorEmail { get; set; }
    public bool NotificarPorSMS { get; set; }
    public bool NotificacionInmediata { get; set; }
    public TimeOnly? HoraInicioSilencioso { get; set; }
    public TimeOnly? HoraFinSilencioso { get; set; }
}

public class CreateNotificationDto
{
    public int UsuarioId { get; set; }
    public string TipoNotificacion { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public int? IncidenteId { get; set; }
}