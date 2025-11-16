using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _service.GetAllUsersAsync();
            return StatusCode(response.ResCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");
            
            var response = await _service.GetUserByIdAsync(id);
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

            var response = await _service.CreateUserAsync(dto);

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

            var response = await _service.UpdateUserAsync(id, dto);
            return StatusCode(response.ResCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _service.DeleteUserAsync(id);
            return StatusCode(response.ResCode, response);
        }

    }
}
