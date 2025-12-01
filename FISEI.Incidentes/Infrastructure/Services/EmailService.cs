using System.Net;
using System.Net.Mail;

namespace FISEI.Incidentes.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection.GetValue<string>("Host");
            var port = smtpSection.GetValue<int>("Port");
            var user = smtpSection.GetValue<string>("User");
            var pass = smtpSection.GetValue<string>("Pass");
            var from = smtpSection.GetValue<string>("From");
            var enableSsl = smtpSection.GetValue<bool>("EnableSsl", true);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };
            using var message = new MailMessage(from!, toEmail)
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            await client.SendMailAsync(message);
        }
    }
}
