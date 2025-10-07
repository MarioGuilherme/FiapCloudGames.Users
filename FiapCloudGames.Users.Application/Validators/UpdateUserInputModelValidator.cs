using FiapCloudGames.Users.Application.InputModels;
using FluentValidation;
using Serilog;

namespace FiapCloudGames.Users.Application.Validators;

public class UpdateUserInputModelValidator : AbstractValidator<UpdateUserInputModel>
{
    public UpdateUserInputModelValidator()
    {
        Log.Information("Iniciando {ValidatorName}", nameof(UpdateUserInputModelValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("O nome precisa ser informado!")
            .MaximumLength(60).WithMessage("O nome não pode exceder 60 caracteres!");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("O e-mail precisa ser informado!")
            .MaximumLength(60).WithMessage("O e-mail não pode exceder 60 caracteres!")
            .EmailAddress().WithMessage("Informe um e-mail válido!");
    }
}
