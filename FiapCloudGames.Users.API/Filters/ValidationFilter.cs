using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Users.Application.ViewModels;
using Serilog;

namespace FiapCloudGames.Users.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        Log.Warning("Corpo da requisição inválido");
        Dictionary<string, string[]> errors = context.ModelState
            .Where(e => e.Value!.Errors.Count > 0)
            .ToDictionary(
                ms => char.ToLowerInvariant(ms.Key[0]) + ms.Key[1..],
                ms => ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        context.Result = new BadRequestObjectResult(new RestResponse { Errors = errors });
    }
}
