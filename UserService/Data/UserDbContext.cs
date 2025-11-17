using Microsoft.EntityFrameworkCore;
using UserService.Data.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<User> AppUsers { get; set; }

    }
}
