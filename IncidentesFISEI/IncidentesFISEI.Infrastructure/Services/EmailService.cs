using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IncidentesFISEI.Infrastructure.Services;

namespace IncidentesFISEI.Infrastructure.Services;

/// <summary>
/// Servicio para el envío de correos electrónicos SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpClient = ConfigureSmtpClient();
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string mensaje)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _configuration["Email:FromAddress"] ?? "noreply@fisei.uta.edu.ec",
                    _configuration["Email:FromName"] ?? "Sistema de Incidentes FISEI"
                ),
                Subject = asunto,
                Body = GenerarCuerpoHtml(mensaje),
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            await _smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation($"Email enviado exitosamente a {destinatario}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando email a {destinatario}: {ex.Message}");
            return false;
        }
    }

    private SmtpClient ConfigureSmtpClient()
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            client.Credentials = new NetworkCredential(username, password);
        }

        return client;
    }

    private string GenerarCuerpoHtml(string mensaje)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Sistema de Incidentes FISEI</title>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            color: #333; 
            max-width: 600px; 
            margin: 0 auto; 
            padding: 20px; 
        }}
        .header {{ 
            background-color: #0056b3; 
            color: white; 
            padding: 20px; 
            text-align: center; 
            border-radius: 8px 8px 0 0; 
        }}
        .content {{ 
            background-color: #f8f9fa; 
            padding: 20px; 
            border: 1px solid #dee2e6; 
        }}
        .footer {{ 
            background-color: #6c757d; 
            color: white; 
            padding: 15px; 
            text-align: center; 
            font-size: 12px; 
            border-radius: 0 0 8px 8px; 
        }}
        .button {{ 
            display: inline-block; 
            padding: 12px 24px; 
            background-color: #28a745; 
            color: white; 
            text-decoration: none; 
            border-radius: 4px; 
            margin: 10px 0; 
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>🎓 FISEI - Universidad Técnica de Ambato</h2>
        <p>Sistema de Gestión de Incidentes ITIL v3</p>
    </div>
    
    <div class='content'>
        <p>{mensaje.Replace("\n", "<br>")}</p>
        
        <p style='margin-top: 20px;'>
            <a href='http://localhost:5000' class='button'>Acceder al Sistema</a>
        </p>
    </div>
    
    <div class='footer'>
        <p>Este es un mensaje automático del Sistema de Incidentes FISEI.</p>
        <p>No responda a este correo. Para soporte contacte: soporte@fisei.uta.edu.ec</p>
        <p>© 2024 FISEI - Facultad de Ingeniería en Sistemas, Electrónica e Industrial</p>
    </div>
</body>
</html>";
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
    }
}

/// <summary>
/// Servicio simulado para el envío de SMS
/// En un entorno de producción se integraría con providers como Twilio, AWS SNS, etc.
/// </summary>
public class SMSService : ISMSService
{
    private readonly ILogger<SMSService> _logger;
    private readonly IConfiguration _configuration;

    public SMSService(ILogger<SMSService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> EnviarSMSAsync(string numeroTelefono, string mensaje)
    {
        try
        {
            // En desarrollo, solo registramos el SMS que se enviaría
            var mensajeLog = $"SMS a {numeroTelefono}: {mensaje}";
            
            if (_configuration["SMS:SimulationMode"] == "true")
            {
                _logger.LogInformation($"[SIMULACIÓN SMS] {mensajeLog}");
                
                // Simular delay de envío
                await Task.Delay(500);
                return true;
            }
            else
            {
                // Aquí iría la integración real con el proveedor de SMS
                // Por ejemplo, con Twilio:
                /*
                var accountSid = _configuration["SMS:Twilio:AccountSid"];
                var authToken = _configuration["SMS:Twilio:AuthToken"];
                var fromNumber = _configuration["SMS:Twilio:FromNumber"];
                
                TwilioClient.Init(accountSid, authToken);
                
                var messageResult = await MessageResource.CreateAsync(
                    body: mensaje,
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(numeroTelefono)
                );
                
                return messageResult.Status != MessageResource.StatusEnum.Failed;
                */
                
                _logger.LogWarning("SMS Service no configurado para producción");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando SMS a {numeroTelefono}: {ex.Message}");
            return false;
        }
    }
}