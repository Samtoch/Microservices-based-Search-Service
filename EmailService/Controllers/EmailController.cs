using Microsoft.AspNetCore.Mvc;
using EmailService.Services;
using EmailService.Models;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-signup-notification")]
        public IActionResult SendSignupNotification([FromBody] SignupRequest request)
        {
            _emailService.SendSignupEmail(request.Email, request.Username);
            return Ok("Signup email sent.");
        }

        [HttpPost("send-password-reset-notification")]
        public IActionResult SendPasswordResetNotification([FromBody] PasswordResetRequest request)
        {
            _emailService.SendPasswordResetEmail(request.Email, request.Token);
            return Ok("Password reset email sent.");
        }
    }
}