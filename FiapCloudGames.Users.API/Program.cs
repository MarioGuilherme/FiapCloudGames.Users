using FiapCloudGames.Users.API.Filters;
using FiapCloudGames.Users.API.Middlewares;
using FiapCloudGames.Users.Application;
using FiapCloudGames.Users.Infrastructure;
using FiapCloudGames.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] (CorrelationId={CorrelationId}) {Message:lj} {NewLine}{Exception}")
    .WriteTo.NewRelicLogs(
        endpointUrl: "https://log-api.newrelic.com/log/v1",
        insertKey: "3F76176BC7D48FB62A252DB7AB0C62708B59B3DCA903928062C304AE327DF776",
        licenseKey: "7a51751afd870f032bf67a4ad3600db4FFFFNRAL",
        applicationName: "FiapCloudGames.Users")
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration);

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API - Fiap Cloud Games - Users",
        Description = "Developed by Mário Guilherme de Andrade Rodrigues",
        Version = "v1",
        Contact = new()
        {
            Name = "Mário Guilherme de Andrade Rodrigues",
            Email = "marioguilhermedev@gmail.com"
        },
        License = new()
        {
            Name = "MIT",
            Url = new("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new() { {
        new() {
            Reference = new() {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        },
        Array.Empty<string>()
    }});
});

WebApplication app = builder.Build();

#region Garante a criação do banco de dados na inicialização
try
{
    using AsyncServiceScope asyncServiceScope = app.Services.CreateAsyncScope();
    IServiceProvider services = asyncServiceScope.ServiceProvider;
    FiapCloudGamesUsersDbContext context = services.GetRequiredService<FiapCloudGamesUsersDbContext>();
    if ((await context.Database.GetPendingMigrationsAsync()).Any())
        await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro durante inicialização do banco!");
}
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GatewayAuthMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

Log.CloseAndFlush();
