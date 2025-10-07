namespace FiapCloudGames.Users.Application.InputModels;

public class UpdateUserInputModel(int userId, UserInputModel userInputModel) : UserInputModel(
    userInputModel.Name,
    userInputModel.Email
)
{
    public int UserId { get; private set; } = userId;
}
