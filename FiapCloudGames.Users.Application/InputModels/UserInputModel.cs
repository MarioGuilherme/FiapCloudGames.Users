namespace FiapCloudGames.Users.Application.InputModels;

public class UserInputModel(string name, string email)
{
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
}
