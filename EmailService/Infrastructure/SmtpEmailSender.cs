using System.Net;
using System.Net.Mail;
using EmailService.Config;

namespace EmailService.Infrastructure
{
    public class SmtpEmailSender
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSender(EmailSettings settings)
        {
            _settings = settings;
        }

        public void SendEmail(string to, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(_settings.From, to, subject, body);
            client.Send(mailMessage);
        }
    }
}