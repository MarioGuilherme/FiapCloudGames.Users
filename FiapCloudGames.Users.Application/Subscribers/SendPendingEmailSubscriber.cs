using FiapCloudGames.Users.Domain.Entities;
using FiapCloudGames.Users.Domain.Events;
using FiapCloudGames.Users.Domain.Repositories;
using FiapCloudGames.Users.Domain.Services;
using FiapCloudGames.Users.Infrastructure.Messaging.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Users.Application.Subscribers;

public class SendPendingEmailSubscriber(IServiceProvider serviceProvider, IOptions<RabbitMqOptions> options, IRabbitMqConnection connection) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly RabbitMqOptions _options = options.Value;
    private readonly IRabbitMqConnection _connection = connection;
    private const string QUEUE = "send-pending-email";
    private const string ROUTING_KEY = "send.pending.email";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = await (await _connection.GetConnectionAsync()).CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QUEUE, true, false, false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QUEUE, _options.Exchange, ROUTING_KEY, cancellationToken: stoppingToken);
        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());

            SendPendingEmailEvent sendPendingEmailEvent = JsonSerializer.Deserialize<SendPendingEmailEvent>(message)!;

            using (LogContext.PushProperty("CorrelationId", ea.BasicProperties.CorrelationId))
            {
                await ProcessSendPendingEmailAsync(sendPendingEmailEvent);
            }

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(QUEUE, false, consumer, cancellationToken: stoppingToken);
    }

    private async Task ProcessSendPendingEmailAsync(SendPendingEmailEvent sendPendingEmailEvent)
    {
        Log.Information("Subscriber {SubscriberName} iniciado às {DateTime}", nameof(SendPendingEmailSubscriber), DateTime.Now);

        using IServiceScope scope = _serviceProvider.CreateScope();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        User user = (await userRepository.GetByIdTrackingAsync(sendPendingEmailEvent.UserId))!;

        await emailService.SendEmailAsync(user.Email, sendPendingEmailEvent.Subject, sendPendingEmailEvent.HtmlContent);

        Log.Information("Subscriber {SubscriberName} finalizado às {DateTime}", nameof(SendPendingEmailSubscriber), DateTime.Now);
    }
}
