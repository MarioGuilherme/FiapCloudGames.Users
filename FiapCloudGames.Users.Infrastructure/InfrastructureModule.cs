using FiapCloudGames.Users.Domain.Repositories;
using FiapCloudGames.Users.Infrastructure.Persistence;
using FiapCloudGames.Users.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Users.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = "";// configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<FiapCloudGamesUsersDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
