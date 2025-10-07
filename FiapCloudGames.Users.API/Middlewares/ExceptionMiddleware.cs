using FiapCloudGames.Users.Domain.Exceptions;
using Serilog;

namespace FiapCloudGames.Users.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = ex switch
            {
                UserNotFoundException => StatusCodes.Status404NotFound,
                InvalidFormException => StatusCodes.Status400BadRequest,
                EmailAlreadyInUseException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
                Log.Error(ex, "Erro interno no serviço FiapCloudGames.Users");
        }
    }
}