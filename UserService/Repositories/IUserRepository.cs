using UserService.Data.Models;

namespace UserService.Repositories
{
    public interface IUserRepository
    {
        Task<int> CountAsync();
        Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        //Task DeleteAsync(User user);
        Task SaveChangesAsync();
    }
}
