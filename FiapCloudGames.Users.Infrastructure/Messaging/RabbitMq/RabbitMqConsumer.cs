using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FiapCloudGames.Users.Infrastructure.Messaging.RabbitMq;

public class RabbitMqConsumer(IOptions<RabbitMqOptions> options, IRabbitMqConnection connection) : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;
    private readonly IRabbitMqConnection _connection = connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = await (await _connection.GetConnectionAsync()).CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, true, cancellationToken: stoppingToken);

        foreach (string queueName in _options.Queues)
        {
            await channel.QueueDeclareAsync(queueName, true, false, false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queueName, _options.Exchange, _options.RoutingKey, cancellationToken: stoppingToken);
            AsyncEventingBasicConsumer consumer = new(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Mensagem recebida: {message}");
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken: stoppingToken);
        }
    }
}
