using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Application.Services;
using FiapCloudGames.Users.Application.ViewModels;
using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Enums;
using FiapCloudGames.Users.Domain.Exceptions;
using FiapCloudGames.Users.Domain.Repositories;
using Moq;

namespace FiapCloudGames.Users.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly IUserService _userService;

    public UserServiceTests() => this._userService = new UserService(_repository.Object, _authService.Object);

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserFound()
    {
        // Arrange
        User user = new(1, "Usuário 1", "usuario1@gmail.com", "senhaForte1", UserType.User);
        this._repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        RestResponse<UserViewModel> restResponse = await this._userService.GetByIdAsync(It.IsAny<int>());

        // Assert
        Assert.NotNull(restResponse);
        Assert.False(restResponse.Errors?.Any() ?? false);
    }

    [Fact]
    public async Task UpdateById_ShouldUpdate_WhenUserFound()
    {
        // Arrange
        User user = new(1, "Usuário 1", "usuario1@gmail.com", "senhaForte1", UserType.User);
        this._repository.Setup(r => r.GetByIdTrackingAsync(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        RestResponse<UserViewModel> restResponse = await this._userService.UpdateUserAsync(new(1, new("Usuário 32", "usuario32@gmail.com")));

        // Assert
        this._repository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        Assert.False(restResponse.Errors?.Any() ?? false);
        Assert.NotNull(restResponse.Data);
    }

    [Fact]
    public async Task UpdateUserById_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        UpdateUserInputModel updateUserInputModel = new(1, new("Usuário 3", "usuario3@gmail.com"));
        this._repository.Setup(r => r.GetByIdTrackingAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

        // Act - Assert
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await this._userService.UpdateUserAsync(updateUserInputModel));
    }

    [Fact]
    public async Task DeleteById_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        this._repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

        // Act - Assert
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await this._userService.DeleteByUserIdAsync(It.IsAny<int>()));
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoOneExists()
    {
        // Arrange
        IEnumerable<User> users = [];
        this._repository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        RestResponse<IEnumerable<UserViewModel>> restResponse = await this._userService.GetAllAsync();

        // Assert
        Assert.NotNull(restResponse?.Data);
        Assert.True(!restResponse?.Data.Any());
        Assert.False(restResponse!.Errors?.Any() ?? false);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrowException_WhenEmailAlreadyInUse()
    {
        // Arrange
        RegisterUserInputModel registerUserInputModel = new("Usuário novo", "emailJaUsado@gmail.com", "senhaNovaForte");
        this._repository.Setup(r => r.EmailInUseAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act - Assert
        await Assert.ThrowsAsync<EmailAlreadyInUseException>(async () => await this._userService.RegisterAsync(registerUserInputModel));
    }

    [Fact]
    public async Task GetAll_ShouldReturnAll_WhenAnyExists()
    {
        // Arrange
        IEnumerable<User> users = [
            new(1, "Usuário 1", "usuario1@gmail.com", "senhaForte1", UserType.User),
            new(1, "Usuário 2", "usuario2@gmail.com", "senhaForte2", UserType.Admin),
            new(1, "Usuário 3", "usuario3@gmail.com", "senhaForte3", UserType.Admin),
            new(1, "Usuário 4", "usuario4@gmail.com", "senhaForte4", UserType.User),
            new(1, "Usuário 5", "usuario5@gmail.com", "senhaForte5", UserType.Admin),
            new(1, "Usuário 6", "usuario6@gmail.com", "senhaForte6", UserType.User),
            new(1, "Usuário 7", "usuario7@gmail.com", "senhaForte7", UserType.Admin),
        ];
        this._repository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        RestResponse<IEnumerable<UserViewModel>> restResponse = await this._userService.GetAllAsync();

        // Assert
        Assert.Equal("Usuário 6", restResponse.Data!.ElementAt(5).Name);
        Assert.NotNull(restResponse?.Data);
        Assert.False(restResponse.Errors?.Any() ?? false);
    }

    [Fact]
    public async Task DeleteById_ShouldDeleteById_WhenExists()
    {
        // Arrange
        User user = new(1, "Usuário a ser excluído 1", "usuario1@gmail.com", "senhaForte2", UserType.Admin);
        this._repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        // Act
        RestResponse restResponse = await this._userService.DeleteByUserIdAsync(It.IsAny<int>());

        // Assert
        this._repository.Verify(r => r.DeleteAsync(user), Times.Once);
        Assert.False(restResponse.Errors?.Any() ?? false);
        Assert.NotNull(restResponse);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnAccessToken_WhenCreated()
    {
        // Arrange
        RegisterUserInputModel registerUserInputModel = new("Usuário novo", "emailDisponivel@gmail.com", "senhaNovaForte");
        this._repository.Setup(r => r.EmailInUseAsync(It.IsAny<string>())).ReturnsAsync(false);
        this._authService.Setup(a => a.GenerateToken(It.IsAny<User>())).Returns("ey....");

        // Act
        RestResponse<AccessTokenViewModel> restResponse = await this._userService.RegisterAsync(registerUserInputModel);

        // Assert
        Assert.False(restResponse.Errors?.Any() ?? false);
        Assert.NotNull(restResponse?.Data?.AccessToken);
    }

    [Fact]
    public async Task LoginUser_ShouldThrowException_WhenEmailDotNotMatch()
    {
        // Arrange
        LoginInputModel loginInputModel = new("emailCadastrado@gmail.com", "senhaNovaForte");
        this._repository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        // Act - Assert
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await this._userService.LoginAsync(loginInputModel));
    }

    [Fact]
    public async Task GetById_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        this._repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

        // Act - Assert
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await this._userService.GetByIdAsync(It.IsAny<int>()));
    }
}
