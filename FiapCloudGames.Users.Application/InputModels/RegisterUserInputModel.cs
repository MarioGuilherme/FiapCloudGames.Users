using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Enums;
using static BCrypt.Net.BCrypt;

namespace FiapCloudGames.Users.Application.InputModels;

public class RegisterUserInputModel(string name, string email, string password)
{
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;

    public User ToEntity() => new(Name, Email, HashPassword(Password), UserType.User);
}
