
using EmailService.Config;
using EmailService.Infrastructure;
using EmailService.Messaging;
using EmailService.Services;

namespace EmailService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
            var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();

            builder.Services.AddSingleton(emailSettings);
            builder.Services.AddSingleton(rabbitMqSettings);

            builder.Services.AddSingleton<SmtpEmailSender>();
            builder.Services.AddScoped<IEmailService, EmailService.Services.EmailService>();

            // Register RabbitMQ consumer as background service
            builder.Services.AddHostedService<EmailEventConsumer>();

            // Add controllers and Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email Service API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
