using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IncidentesFISEI.Application.Interfaces;

namespace IncidentesFISEI.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true)
    {
        try
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            var fromName = _configuration["EmailSettings:FromName"] ?? "Soporte FISEI";

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogWarning("Configuración de email incompleta. Email no enviado.");
                return false;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = esHtml
            };

            mailMessage.To.Add(destinatario);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email enviado exitosamente a {Destinatario}", destinatario);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Destinatario}", destinatario);
            return false;
        }
    }

    public async Task<bool> EnviarNotificacionIncidenteAsync(string destinatario, string nombreUsuario, 
        int incidenteId, string titulo, string mensaje, string tipoNotificacion)
    {
        var asunto = $"Incidente #{incidenteId} - {tipoNotificacion}";
        var cuerpo = GenerarCuerpoNotificacionIncidente(nombreUsuario, incidenteId, titulo, mensaje, tipoNotificacion);
        
        return await EnviarEmailAsync(destinatario, asunto, cuerpo, true);
    }

    public async Task<bool> EnviarAlertaSLAAsync(string destinatario, string nombreUsuario, 
        int incidenteId, string tituloIncidente, DateTime fechaVencimiento, bool yaVencido)
    {
        var asunto = yaVencido 
            ? $"⚠️ SLA VENCIDO - Incidente #{incidenteId}" 
            : $"⏰ Alerta SLA - Incidente #{incidenteId}";
        
        var cuerpo = GenerarCuerpoAlertaSLA(nombreUsuario, incidenteId, tituloIncidente, fechaVencimiento, yaVencido);
        
        return await EnviarEmailAsync(destinatario, asunto, cuerpo, true);
    }

    public async Task<bool> EnviarNotificacionEscalacionAsync(string destinatario, string nombreUsuario, 
        int incidenteId, string tituloIncidente, string motivo, string nivelEscalacion)
    {
        var asunto = $"🔺 Escalación - Incidente #{incidenteId}";
        var cuerpo = GenerarCuerpoEscalacion(nombreUsuario, incidenteId, tituloIncidente, motivo, nivelEscalacion);
        
        return await EnviarEmailAsync(destinatario, asunto, cuerpo, true);
    }

    private static string GenerarCuerpoNotificacionIncidente(string nombreUsuario, int incidenteId, 
        string titulo, string mensaje, string tipoNotificacion)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; border-radius: 8px; color: white; }}
        .logo {{ font-size: 24px; font-weight: bold; }}
        .content {{ line-height: 1.6; color: #333; }}
        .incident-box {{ background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .incident-id {{ color: #667eea; font-weight: bold; font-size: 18px; }}
        .button {{ display: inline-block; background-color: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center; }}
        .badge {{ display: inline-block; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold; background-color: #667eea; color: white; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""logo"">🎫 Sistema de Incidentes FISEI</div>
            <h2>{tipoNotificacion}</h2>
        </div>
        
        <div class=""content"">
            <p>Hola <strong>{nombreUsuario}</strong>,</p>
            
            <p>{mensaje}</p>
            
            <div class=""incident-box"">
                <p class=""incident-id"">Incidente #{incidenteId}</p>
                <p><strong>Título:</strong> {titulo}</p>
                <p><span class=""badge"">{tipoNotificacion}</span></p>
            </div>
            
            <p style=""text-align: center;"">
                <a href=""http://localhost:5000/incidentes/{incidenteId}"" class=""button"">Ver Incidente</a>
            </p>
            
            <p>Por favor, revisa el incidente y toma las acciones necesarias.</p>
        </div>
        
        <div class=""footer"">
            <p>Este es un mensaje automático del Sistema de Gestión de Incidentes FISEI.</p>
            <p>© 2025 Universidad Técnica de Ambato - Facultad de Ingeniería en Sistemas</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GenerarCuerpoAlertaSLA(string nombreUsuario, int incidenteId, 
        string tituloIncidente, DateTime fechaVencimiento, bool yaVencido)
    {
        var colorAlerta = yaVencido ? "#dc3545" : "#ffc107";
        var icono = yaVencido ? "⚠️" : "⏰";
        var titulo = yaVencido ? "SLA VENCIDO" : "Alerta de SLA Próximo a Vencer";
        var mensaje = yaVencido 
            ? $"El SLA del incidente #{incidenteId} ha vencido. Se requiere acción inmediata."
            : $"El SLA del incidente #{incidenteId} está próximo a vencer el {fechaVencimiento:dd/MM/yyyy HH:mm}.";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; background-color: {colorAlerta}; padding: 20px; border-radius: 8px; color: white; }}
        .alert-box {{ background-color: #fff3cd; border-left: 4px solid {colorAlerta}; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .button {{ display: inline-block; background-color: {colorAlerta}; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>{icono} {titulo}</h1>
        </div>
        
        <div class=""content"">
            <p>Hola <strong>{nombreUsuario}</strong>,</p>
            
            <div class=""alert-box"">
                <p><strong>{mensaje}</strong></p>
                <p><strong>Incidente #{incidenteId}:</strong> {tituloIncidente}</p>
                <p><strong>Fecha Vencimiento:</strong> {fechaVencimiento:dd/MM/yyyy HH:mm}</p>
            </div>
            
            <p style=""text-align: center;"">
                <a href=""http://localhost:5000/incidentes/{incidenteId}"" class=""button"">Ver Incidente Ahora</a>
            </p>
        </div>
        
        <div class=""footer"">
            <p>© 2025 Sistema de Gestión de Incidentes FISEI - UTA</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GenerarCuerpoEscalacion(string nombreUsuario, int incidenteId, 
        string tituloIncidente, string motivo, string nivelEscalacion)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%); padding: 20px; border-radius: 8px; color: white; }}
        .escalation-box {{ background-color: #fff3f3; border-left: 4px solid #ff6b6b; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .button {{ display: inline-block; background-color: #ff6b6b; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔺 Escalación de Incidente</h1>
        </div>
        
        <div class=""content"">
            <p>Hola <strong>{nombreUsuario}</strong>,</p>
            
            <p>Se ha escalado un incidente que requiere tu atención:</p>
            
            <div class=""escalation-box"">
                <p><strong>Incidente #{incidenteId}:</strong> {tituloIncidente}</p>
                <p><strong>Nivel de Escalación:</strong> {nivelEscalacion}</p>
                <p><strong>Motivo:</strong> {motivo}</p>
            </div>
            
            <p style=""text-align: center;"">
                <a href=""http://localhost:5000/incidentes/{incidenteId}"" class=""button"">Atender Incidente</a>
            </p>
        </div>
        
        <div class=""footer"">
            <p>© 2025 Sistema de Gestión de Incidentes FISEI - UTA</p>
        </div>
    </div>
</body>
</html>";
    }
}
