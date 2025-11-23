using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Data.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CountAsync()
        {
            return await _context.AppUsers.CountAsync(u => !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.AppUsers
                 .Where(u => !u.IsDeleted)
                 .OrderBy(u => u.CreatedDate)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all users from database.");
            try
            {
                var users = await _context.AppUsers.ToListAsync();
                _logger.LogInformation("Successfully fetched {Count} users.", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users.");
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {Id}", id);
            try
            {
                var user = await _context.AppUsers.FindAsync(id);
                if (user == null)
                    _logger.LogWarning("User with ID {Id} not found.", id);
                else
                    _logger.LogInformation("User with ID {Id} retrieved successfully.", id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID {Id}.", id);
                throw;
            }
        }

        public async Task AddAsync(User user)
        {
            _logger.LogInformation("Adding new user: {Username}", user.UserName);
            try
            {
                await _context.AppUsers.AddAsync(user);
                _logger.LogInformation("User {Username} added successfully.", user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user {Username}.", user.UserName);
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            _logger.LogInformation("Updating user: {Username}", user.UserName);
            try
            {
                _context.AppUsers.Update(user);
                _logger.LogInformation("User {Username} updated successfully.", user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {Username}.", user.UserName);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to database.");
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Database changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to database.");
                throw;
            }
        }
    }
}
