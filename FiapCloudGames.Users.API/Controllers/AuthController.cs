using FiapCloudGames.Users.Application.InputModels;
using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Users.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginInputModel inputModel)
    {
        RestResponse<AccessTokenViewModel> restResponse = await _userService.LoginAsync(inputModel);
        return Ok(restResponse);
    }

    [HttpPost("register")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserInputModel inputModel)
    {
        RestResponse<AccessTokenViewModel> restResponse = await _userService.RegisterAsync(inputModel);
        return Ok(restResponse);
    }
}
