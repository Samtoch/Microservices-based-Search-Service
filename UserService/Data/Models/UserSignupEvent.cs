namespace UserService.Data.Models
{
    public class UserSignupEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
