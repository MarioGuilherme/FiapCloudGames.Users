using RabbitMQ.Client;

namespace FiapCloudGames.Users.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqConnection
{
    Task<IConnection> GetConnectionAsync();
}
