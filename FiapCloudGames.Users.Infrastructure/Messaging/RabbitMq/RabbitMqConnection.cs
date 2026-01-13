using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FiapCloudGames.Users.Infrastructure.Messaging.RabbitMq;

public class RabbitMqConnection(IOptions<RabbitMqOptions> options) : IRabbitMqConnection
{
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        ConnectionFactory factory = new()
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }
}
