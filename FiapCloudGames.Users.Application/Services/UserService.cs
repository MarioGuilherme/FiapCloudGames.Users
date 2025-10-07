using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Application.ViewModels;
using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Exceptions;
using FiapCloudGames.Users.Domain.Repositories;
using Serilog;
using static BCrypt.Net.BCrypt;

namespace FiapCloudGames.Users.Application.Services;

public class UserService(IUserRepository userRepository, IAuthService authService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuthService _authService = authService;

    public async Task<RestResponse> DeleteByUserIdAsync(int userId)
    {
        Log.Information("Iniciando exclusão do usuário {userId}", userId);
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            Log.Warning("Usuário {userId} não encontrado para exclusão", userId);
            throw new UserNotFoundException();
        }
        await _userRepository.DeleteAsync(user);
        Log.Information("Usuário {userId} excluído com sucesso", userId);
        return RestResponse.Success();
    }

    public async Task<RestResponse<IEnumerable<UserViewModel>>> GetAllAsync()
    {
        Log.Information("Iniciando recuperação de todos os usuários");
        IEnumerable<User> users = await _userRepository.GetAllAsync();
        Log.Information("Recuperação de todos os usuários finalizada. Total de usuários: {count}", users.Count());
        return RestResponse<IEnumerable<UserViewModel>>.Success(users.Select(UserViewModel.FromEntity));
    }

    public async Task<RestResponse<UserViewModel>> GetByIdAsync(int userId)
    {
        Log.Information("Iniciando recuperação do usuário {userId}", userId);
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            Log.Warning("Usuário {userId} não encontrado", userId);
            throw new UserNotFoundException();
        }
        Log.Information("Recuperação do usuário {userId} finalizada", userId);
        return RestResponse<UserViewModel>.Success(UserViewModel.FromEntity(user));
    }

    public async Task<RestResponse<AccessTokenViewModel>> LoginAsync(LoginInputModel inputModel)
    {
        Log.Information("Iniciando login do usuário {email}", inputModel.Email);
        User? user = await _userRepository.GetByEmailAsync(inputModel.Email);
        if (user is null)
        {
            Log.Warning("Usuário com email {email} não encontrado", inputModel.Email);
            throw new UserNotFoundException();
        }

        if (!Verify(inputModel.Password, user.Password))
        {
            Log.Warning("Senha inválida para o usuário {email}", inputModel.Email);
            throw new UserNotFoundException();
        }

        string accessToken = _authService.GenerateToken(user);
        return RestResponse<AccessTokenViewModel>.Success(new(accessToken));
    }

    public async Task<RestResponse<AccessTokenViewModel>> RegisterAsync(RegisterUserInputModel inputModel)
    {
        Log.Information("Iniciando registro do usuário {email}", inputModel.Email);
        if (await _userRepository.EmailInUseAsync(inputModel.Email))
        {
            Log.Warning("Email {email} já está em uso", inputModel.Email);
            throw new EmailAlreadyInUseException();
        }

        User newUser = inputModel.ToEntity();
        await _userRepository.AddAsync(newUser);
        Log.Information("Registro do usuário {email} realizado com sucesso", inputModel.Email);

        string accessToken = _authService.GenerateToken(newUser);
        return RestResponse<AccessTokenViewModel>.Success(new(accessToken));
    }

    public async Task<RestResponse<UserViewModel>> UpdateUserAsync(UpdateUserInputModel inputModel)
    {
        Log.Information("Iniciando atualização do usuário {userId}", inputModel.UserId);
        User? user = await _userRepository.GetByIdTrackingAsync(inputModel.UserId);
        if (user is null)
        {
            Log.Warning("Usuário {userId} não encontrado para atualização", inputModel.UserId);
            throw new UserNotFoundException();
        }

        if (await _userRepository.EmailInUseAsync(inputModel.Email))
        {
            Log.Warning("Email {email} já está em uso", inputModel.Email);
            throw new EmailAlreadyInUseException();
        }

        user.Update(inputModel.Name, inputModel.Email);
        await _userRepository.UpdateAsync(user);
        Log.Information("Atualização do usuário {userId} realizada com sucesso", inputModel.UserId);

        return RestResponse<UserViewModel>.Success(UserViewModel.FromEntity(user));
    }
}
