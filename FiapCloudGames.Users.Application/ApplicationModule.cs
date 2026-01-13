using FiapCloudGames.Users.Application;
using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Application.Services;
using FiapCloudGames.Users.Application.Subscribers;
using FiapCloudGames.Users.Application.Validators;
using FiapCloudGames.Users.Domain.Services;
using FiapCloudGames.Users.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FiapCloudGames.Users.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddEmailService(configuration)
            .AddSubscribers()
            .AddAuthentication(configuration)
            .AddFluentValidation()
            .AddApplicationServices();

        return services;
    }

    private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEmailService, SendGridEmailService>(_ =>
        {
            string apiKey = configuration.GetValue<string>("SendGrid:ApiKey")!;
            string senderEmail = configuration.GetValue<string>("SendGrid:SenderEmail")!;
            return new(apiKey, senderEmail);
        });

        return services;
    }

    private static IServiceCollection AddSubscribers(this IServiceCollection services)
    {
        services.AddHostedService<SendPendingEmailSubscriber>();
        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation(o => o.DisableDataAnnotationsValidation = true)
            .AddValidatorsFromAssemblyContaining<UpdateUserInputModelValidator>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                };
            });

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
