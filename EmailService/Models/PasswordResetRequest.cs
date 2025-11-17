namespace EmailService.Models
{
    public class PasswordResetRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
