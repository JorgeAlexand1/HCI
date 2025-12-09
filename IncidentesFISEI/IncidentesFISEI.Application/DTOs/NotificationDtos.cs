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

public class PlantillaNotificacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoNotificacion { get; set; } = string.Empty;
    public string PlantillaTitulo { get; set; } = string.Empty;
    public string PlantillaMensaje { get; set; } = string.Empty;
    public string? PlantillaEmail { get; set; }
    public string? PlantillaSMS { get; set; }
    public bool IsActive { get; set; }
    public string? VariablesDisponibles { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePlantillaNotificacionDto
{
    public string Nombre { get; set; } = string.Empty;
    public string TipoNotificacion { get; set; } = string.Empty;
    public string PlantillaTitulo { get; set; } = string.Empty;
    public string PlantillaMensaje { get; set; } = string.Empty;
    public string? PlantillaEmail { get; set; }
    public string? PlantillaSMS { get; set; }
    public string? VariablesDisponibles { get; set; }
}

public class UpdatePlantillaNotificacionDto
{
    public string Nombre { get; set; } = string.Empty;
    public string PlantillaTitulo { get; set; } = string.Empty;
    public string PlantillaMensaje { get; set; } = string.Empty;
    public string? PlantillaEmail { get; set; }
    public string? PlantillaSMS { get; set; }
    public bool IsActive { get; set; }
    public string? VariablesDisponibles { get; set; }
}

public class LogNotificacionDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string Canal { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Error { get; set; }
}

public class ConfiguracionCanalesNotificacionDto
{
    public bool EmailHabilitado { get; set; }
    public bool SMSHabilitado { get; set; }
    public bool SistemaHabilitado { get; set; }
    public bool NotificacionInmediata { get; set; }
    public bool SonidoAlerta { get; set; }
    public bool NotificacionEscritorio { get; set; }
}