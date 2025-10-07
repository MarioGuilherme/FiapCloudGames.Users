using FiapCloudGames.Users.Domain.Entities;

namespace FiapCloudGames.Users.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task DeleteAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailInUseAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdTrackingAsync(int id);
    Task UpdateAsync(User user);
}
