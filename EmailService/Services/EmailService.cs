using EmailService.Infrastructure;

namespace EmailService.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpEmailSender _smtpSender;

        public EmailService(SmtpEmailSender smtpSender)
        {
            _smtpSender = smtpSender;
        }

        public void SendSignupEmail(string email, string username)
        {
            var subject = "Welcome to Our Platform!";
            var body = $"Hello {username}, thank you for signing up!";
            _smtpSender.SendEmail(email, subject, body);
        }

        public void SendPasswordResetEmail(string email, string token)
        {
            var subject = "Password Reset Request";
            var body = $"Click here to reset your password: https://example.com/reset?token={token}";
            _smtpSender.SendEmail(email, subject, body);
        }
    }
}