using EmailService.Config;
using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EmailService.Messaging
{
    public class EmailEventConsumer : BackgroundService
    {
        private readonly IEmailService _emailService;
        private readonly RabbitMqSettings _settings;

        public EmailEventConsumer(IEmailService emailService, RabbitMqSettings settings)
        {
            _emailService = emailService;
            _settings = settings;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("email.signup", durable: true, exclusive: false, autoDelete: false);
            channel.QueueDeclare("email.passwordreset", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (ea.RoutingKey == "email.signup")
                {
                    var signupEvent = JsonSerializer.Deserialize<UserSignupEvent>(message);
                    _emailService.SendSignupEmail(signupEvent.Email, signupEvent.Username);
                }
                else if (ea.RoutingKey == "email.passwordreset")
                {
                    var resetEvent = JsonSerializer.Deserialize<PasswordResetEvent>(message);
                    _emailService.SendPasswordResetEmail(resetEvent.Email, resetEvent.ResetToken);
                }
            };

            channel.BasicConsume("email.signup", autoAck: true, consumer: consumer);
            channel.BasicConsume("email.passwordreset", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
