
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using UserService.Data;
using UserService.DTOs;
using UserService.Mappings;
using UserService.Middlewares;
using UserService.Repositories;
using UserService.Services;
using UserService.Validators;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting application...");
                var builder = WebApplication.CreateBuilder(args);

                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceConnection")));
                
                builder.Services.AddTransient<IUserService, UserService.Services.UserService>();
                builder.Services.AddTransient<IUserRepository, UserRepository>();

                builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
                builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();

                builder.Services.AddControllers();
                builder.Services.AddOpenApi();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();

                // Global Exception Handling Middleware
                app.UseMiddleware<GlobalExceptionMiddleware>();

                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application stopped due to an exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
