namespace EmailService.Services
{
    public interface IEmailService
    {
        Task SendSignupEmail(string email, string username);
        Task SendPasswordResetEmail(string email, string token);

    }
}
