using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserService.Data.Models;
using UserService.DTOs;
using UserService.Messaging;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;

        public UserController(IUserService userService, IEventPublisher eventPublisher)
        {
            _userService = userService;
            _eventPublisher = eventPublisher;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _userService.GetUsersPagedAsync(pageNumber, pageSize);
            return StatusCode(response.ResCode, response);
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, NoStore = false)]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new ApiResponse<string>
                {
                    ResMsg = "Invalid user ID.",
                    ResCode = 400,
                    ResFlag = false,
                    Data = null
                });

            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode(response.ResCode, response);
        }


        [HttpPost]

        public async Task<IActionResult> CreateUser([FromBody] UserDto dto, [FromServices] IValidator<UserDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            var response = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUser), new { id = response.Data.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto dto, [FromServices] IValidator<UserDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");

            var response = await _userService.UpdateUserAsync(id, dto);
            return StatusCode(response.ResCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return StatusCode(response.ResCode, response);
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);
            _eventPublisher.Publish(new UserSignupEvent
            {
                UserId = user.Data.Id,
                Email = user.Data.Email,
                Username = user.Data.Email
            }, "email.signup");

            return Ok(user);
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset(Guid userId)
        {
            var token = await _userService.GeneratePasswordResetTokenAsync(userId);
            var user = await _userService.GetUserByIdAsync(userId);

            _eventPublisher.Publish(new PasswordResetEvent
            {
                UserId = user.Data.Id,
                Email = user.Data.Email,
                ResetToken = token
            }, "email.passwordreset");

            return Ok("Password reset initiated.");
        }

    }
}
