using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Users.Infrastructure.Persistence.Repositories;

public class UserRepository(FiapCloudGamesUsersDbContext context) : IUserRepository
{
    private readonly FiapCloudGamesUsersDbContext _context = context;

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users
        .AsNoTracking()
        .ToListAsync();

    public Task<User?> GetByEmailAsync(string email) => _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> EmailInUseAsync(string email) => _context.Users
        .AsNoTracking()
        .AnyAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(int id) => _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.UserId == id);

    public Task<User?> GetByIdTrackingAsync(int id) => _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
