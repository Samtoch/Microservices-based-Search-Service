using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Config;

namespace UserService.Messaging
{
    public class RabbitMqPublisher : IEventPublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqPublisher(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        public void Publish<T>(T message, string queueName)
        {
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

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}