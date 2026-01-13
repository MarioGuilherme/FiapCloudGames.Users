using FiapCloudGames.Users.Domain.Repositories;
using FiapCloudGames.Users.Infrastructure.Messaging.RabbitMq;
using FiapCloudGames.Users.Infrastructure.Persistence;
using FiapCloudGames.Users.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Users.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMessageBroker(configuration)
            .AddDbContext()
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        string connectionString = "Server=(localdb)\\mssqllocaldb;Database=FiapCloudGamesUsers;Trusted_Connection=True;MultipleActiveResultSets=true";// Environment.GetEnvironmentVariable("FiapCloudGamesUsersConnectionString")!;
        services.AddDbContext<FiapCloudGamesUsersDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
