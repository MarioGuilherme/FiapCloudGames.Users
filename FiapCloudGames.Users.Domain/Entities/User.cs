using FiapCloudGames.Users.Domain.Enums;

namespace FiapCloudGames.Users.Domain.Entities;

public class User
{
    public int UserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public UserType UserType { get; private set; }
    public string Password { get; private set; }

    public User(int userId, string name, string email, string password, UserType userType)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Password = password;
        UserType = userType;
    }

    public User(string name, string email, string password, UserType userType)
    {
        Name = name;
        Email = email;
        Password = password;
        UserType = userType;
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
