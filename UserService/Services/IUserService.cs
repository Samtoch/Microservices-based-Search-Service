using UserService.Data.Models;
using UserService.DTOs;

namespace UserService.Services
{
    public interface IUserService
    {

        Task<ApiResponse<IEnumerable<User?>>> GetAllUsersAsync();
        Task<ApiResponse<User?>> GetUserByIdAsync(Guid id);
        Task<ApiResponse<User>> CreateUserAsync(UserDto dto);
        Task<ApiResponse<bool>> UpdateUserAsync(Guid id, UserDto dto);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid id);

    }
}
