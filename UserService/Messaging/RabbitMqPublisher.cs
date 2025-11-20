using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Infrastructure.Configuration;

namespace UserService.Messaging
{
    public class RabbitMqPublisher : IEventPublisher
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(RabbitMqSettings settings, ILogger<RabbitMqPublisher> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void Publish<T>(T message, string queueName)
        {
            try
            {
                _logger.LogInformation("Attempting to publish message to queue: {QueueName}", queueName);

                var factory = new ConnectionFactory()
                {
                    HostName = _settings.HostName,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,
                    Port = _settings.Port
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _logger.LogInformation("Queue declared: {QueueName}", queueName);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                _logger.LogInformation("Message published successfully to queue: {QueueName}", queueName);
                _logger.LogDebug("Message content: {Message}", JsonSerializer.Serialize(message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to queue: {QueueName}", queueName);
                throw;
            }
        }
    }
}