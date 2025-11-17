namespace EmailService.Services
{
    public interface IEmailService
    {
        void SendSignupEmail(string email, string username);
        void SendPasswordResetEmail(string email, string token);

    }
}
