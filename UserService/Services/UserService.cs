using AutoMapper;
using Microsoft.Extensions.Logging;
using NLog;
using UserService.Data.Models;
using UserService.DTOs;
using UserService.Middlewares;
using UserService.Repositories;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(ILogger<GlobalExceptionMiddleware> logger, IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<User?>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all user records from the database.");
            var users = await _repository.GetAllAsync();
            return new ApiResponse<IEnumerable<User?>>
            {
                ResMsg = "Records fetched successfully",
                ResCode = 200,
                ResFlag = true,
                Data = users
            };
        }

        public async Task<ApiResponse<User?>> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation($"Fetching user record with ID: {id}");
            var user = await _repository.GetByIdAsync(id);
            return new ApiResponse<User?>
            {
                ResMsg = "Record fetched successfully",
                ResCode = 200,
                ResFlag = true,
                Data = user
            };
        }

        public async Task<ApiResponse<User>> CreateUserAsync(UserDto dto)
        {
            _logger.LogInformation("Creating a new user record.");
            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid();
            user.Email = dto.UserName;
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            return new ApiResponse<User>
            {
                ResMsg = "User created successfully",
                ResCode = 200,
                ResFlag = true,
                Data = user
            };
        }

        public async Task<ApiResponse<bool>> UpdateUserAsync(Guid id, UserDto dto)
        {
            _logger.LogInformation($"Updating user record with ID: {id}");
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    ResMsg = "User not found",
                    ResCode = 204,
                    ResFlag = false,
                    Data = false
                };
            }

            _mapper.Map(dto, user);
            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();
            return new ApiResponse<bool>
            {
                ResMsg = "User deleted successfully",
                ResCode = 200,
                ResFlag = true,
                Data = true
            };
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation($"Deleting user record with ID: {id}");
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    ResMsg = "User not found",
                    ResCode = 204,
                    ResFlag = false,
                    Data = false
                };
            }

            user.IsDeleted = true;
            user.ModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                ResMsg = "User deleted successfully",
                ResCode = 200,
                ResFlag = true,
                Data = true
            };
        }
    }

}
