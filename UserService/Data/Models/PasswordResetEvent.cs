namespace UserService.Data.Models
{
    public class PasswordResetEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string ResetToken { get; set; }
    }
}
