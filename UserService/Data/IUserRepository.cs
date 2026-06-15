using UserService.Models;

namespace UserService.Data;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByNameAsync(string name);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
}
