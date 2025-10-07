using FiapCloudGames.Users.API.Extensions;
using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Application.ViewModels;
using FiapCloudGames.Users.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Users.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(UserType.Admin))]
    public async Task<IActionResult> GetAll()
    {
        RestResponse<IEnumerable<UserViewModel>> restResponse = await _userService.GetAllAsync();
        return Ok(restResponse);
    }

    [HttpGet("{userId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(UserType.Admin))]
    public async Task<IActionResult> GetById(int userId)
    {
        RestResponse<UserViewModel> restResponse = await _userService.GetByIdAsync(userId);
        return Ok(restResponse);
    }

    [HttpGet("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        RestResponse<UserViewModel> restResponse = await _userService.GetByIdAsync(User.UserId());
        return Ok(restResponse);
    }

    [HttpPut("{userId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(UserType.Admin))]
    public async Task<IActionResult> UpdateById(int userId, [FromBody] UserInputModel inputModel)
    {
        RestResponse<UserViewModel> restResponse = await _userService.UpdateUserAsync(new(userId, inputModel));
        return Ok(restResponse);
    }

    [HttpDelete("{userId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = nameof(UserType.Admin))]
    public async Task<IActionResult> DeleteById(int userId)
    {
        await _userService.DeleteByUserIdAsync(userId);
        return NoContent();
    }
}
