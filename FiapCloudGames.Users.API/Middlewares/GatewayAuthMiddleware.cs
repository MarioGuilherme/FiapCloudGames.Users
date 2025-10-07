using Microsoft.Extensions.Primitives;
using Serilog;

namespace FiapCloudGames.Users.API.Middlewares;

public class GatewayAuthMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private const string InternalHeader = "X-Internal-Auth";
    private readonly string Secret = configuration.GetValue<string>("GatewayInternalAuth")!;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(InternalHeader, out StringValues value) || value != Secret)
        {
            Log.Warning("Tentativa de acesso ao serviço FiapCloudGames.Users fora da API Gateway");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}
