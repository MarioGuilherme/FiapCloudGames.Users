using FiapCloudGames.Users.Domain.Entities;

namespace FiapCloudGames.Users.Application.Interfaces;

public interface IAuthService
{
    string GenerateToken(User user);
}
