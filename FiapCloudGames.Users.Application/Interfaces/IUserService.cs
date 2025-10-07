using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.ViewModels;

namespace FiapCloudGames.Users.Application.Interfaces;

public interface IUserService
{
    Task<RestResponse> DeleteByUserIdAsync(int userId);
    Task<RestResponse<IEnumerable<UserViewModel>>> GetAllAsync();
    Task<RestResponse<UserViewModel>> GetByIdAsync(int userId);
    Task<RestResponse<UserViewModel>> UpdateUserAsync(UpdateUserInputModel inputModel);
    Task<RestResponse<AccessTokenViewModel>> LoginAsync(LoginInputModel inputModel);
    Task<RestResponse<AccessTokenViewModel>> RegisterAsync(RegisterUserInputModel inputModel);
}
