using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.Validators;
using FluentValidation.Results;

namespace FiapCloudGames.Users.Tests.Validators;

public class InputModelValidators
{
    [Fact]
    public void RegisterInputModelValidate_ShouldBeInvalid_WhenPasswordIsInvalid()
    {
        // Arrange
        RegisterUserInputModel registerUserInputModel = new("Nome válido", "emailValido@gmail.com", "123456");

        // Act
        ValidationResult result = new RegisterUserInputModelValidator().Validate(registerUserInputModel);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("A senha deve ter no mínimo 8 caracteres e conter letras, números e caracteres especiais.", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void UpdateInputModelValidate_ShouldBeInvalid_WhenEmailIsInvalid()
    {
        // Arrange
        UpdateUserInputModel updateUserInputModel = new(1, new("Nome válido", "emailInvalidogmail.com"));

        // Act
        ValidationResult result = new UpdateUserInputModelValidator().Validate(updateUserInputModel);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("Informe um e-mail válido!", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void LoginInputModelValidate_ShouldBeValid_WhenAllIsValid()
    {
        // Arrange
        LoginInputModel loginInputModel = new("emailValido@gmail.com", "asd56$Ád");

        // Act
        ValidationResult result = new LoginInputModelValidator().Validate(loginInputModel);

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.Errors.Count == 0);
    }
}
