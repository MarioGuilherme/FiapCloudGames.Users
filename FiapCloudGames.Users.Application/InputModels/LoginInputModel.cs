namespace FiapCloudGames.Users.Application.InputModels;

public class LoginInputModel(string email, string password)
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
}
