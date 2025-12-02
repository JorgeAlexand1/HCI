namespace IncidentesFISEI.Application.Interfaces;

public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true);
    Task<bool> EnviarNotificacionIncidenteAsync(string destinatario, string nombreUsuario, int incidenteId, string titulo, string mensaje, string tipoNotificacion);
    Task<bool> EnviarAlertaSLAAsync(string destinatario, string nombreUsuario, int incidenteId, string tituloIncidente, DateTime fechaVencimiento, bool yaVencido);
    Task<bool> EnviarNotificacionEscalacionAsync(string destinatario, string nombreUsuario, int incidenteId, string tituloIncidente, string motivo, string nivelEscalacion);
}
