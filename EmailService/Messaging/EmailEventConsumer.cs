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
        private readonly ILogger<EmailEventConsumer> _logger;
        private IConnection _connection;
        private IModel _channel;

        public EmailEventConsumer(IEmailService emailService, RabbitMqSettings settings, ILogger<EmailEventConsumer> logger)
        {
            _emailService = emailService;
            _settings = settings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await InitializeRabbitMQ();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation("Received message from queue: {QueueName}", ea.RoutingKey);
                    _logger.LogDebug("Message content: {Message}", message);

                    if (ea.RoutingKey == "email.signup")
                    {
                        var signupEvent = JsonSerializer.Deserialize<UserSignupEvent>(message);
                        if (signupEvent != null)
                        {
                            await _emailService.SendSignupEmail(signupEvent.Email, signupEvent.Username);
                            _logger.LogInformation("Successfully processed signup email for: {Email}", signupEvent.Email);
                        }
                    }
                    else if (ea.RoutingKey == "email.passwordreset")
                    {
                        var resetEvent = JsonSerializer.Deserialize<PasswordResetEvent>(message);
                        if (resetEvent != null)
                        {
                            await _emailService.SendPasswordResetEmail(resetEvent.Email, resetEvent.ResetToken);
                            _logger.LogInformation("Successfully processed password reset email for: {Email}", resetEvent.Email);
                        }
                    }

                    // Manual acknowledgement after successful processing
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue: {QueueName}", ea.RoutingKey);
                    // Reject the message and don't requeue it (send to dead letter queue if configured)
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            // Start consuming with manual acknowledgement
            _channel.BasicConsume("email.signup", autoAck: false, consumer: consumer);
            _channel.BasicConsume("email.passwordreset", autoAck: false, consumer: consumer);

            _logger.LogInformation("EmailEventConsumer started and listening for messages...");

            // Keep the service running until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }


        private async Task InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,
                    DispatchConsumersAsync = true // Important for async processing
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare queues with the same parameters as publisher
                _channel.QueueDeclare(queue: "email.signup",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueDeclare(queue: "email.passwordreset",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                _logger.LogInformation("RabbitMQ connection established and queues declared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
                throw;
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
