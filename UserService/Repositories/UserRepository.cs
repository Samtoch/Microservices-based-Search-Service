using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Data.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
        public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);
        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
        public async Task UpdateAsync(User user) => _context.Users.Update(user);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    }
}
