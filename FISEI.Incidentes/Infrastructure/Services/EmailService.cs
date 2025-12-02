using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace FISEI.Incidentes.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection.GetValue<string>("Host");
            var port = smtpSection.GetValue<int>("Port");
            var user = smtpSection.GetValue<string>("User");
            var pass = smtpSection.GetValue<string>("Pass");
            var from = smtpSection.GetValue<string>("From");

            // Validar configuración
            if (string.IsNullOrWhiteSpace(host) || 
                string.IsNullOrWhiteSpace(user) || 
                string.IsNullOrWhiteSpace(pass) ||
                user.Contains("TU_CORREO", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Configuración SMTP incompleta. Email no enviado a {ToEmail}. Subject: {Subject}", toEmail, subject);
                _logger.LogInformation("Contenido del email que se enviaría:\n{HtmlBody}", htmlBody);
                return;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("FISEI Incidentes", from));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    
                    // Autenticar
                    await client.AuthenticateAsync(user, pass);
                    
                    // Enviar email
                    await client.SendAsync(message);
                    
                    // Desconectar
                    await client.DisconnectAsync(true);
                }
                
                _logger.LogInformation("Email enviado exitosamente a {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {ToEmail}. Detalles: {Message}", toEmail, ex.Message);
                throw; // Lanzar excepción para que el controlador pueda manejarla
            }
        }
    }
}
