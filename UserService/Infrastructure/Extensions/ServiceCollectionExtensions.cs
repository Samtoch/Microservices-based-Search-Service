// Infrastructure/Extensions/ServiceCollectionExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Data;

public static class ServiceCollectionExtensions
{
    //public static IServiceCollection AddAppSettings(this IServiceCollection services)
    //{
    //    // Email Settings
    //    services.Configure<EmailSettings>(options =>
    //    {
    //        options.Server = Environment.GetEnvironmentVariable("EMAIL_SERVER");
    //        options.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT"));
    //        options.Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
    //        options.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
    //        options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL"));
    //    });

    //    // Qdrant Settings
    //    services.Configure<QdrantSettings>(options =>
    //    {
    //        options.Url = Environment.GetEnvironmentVariable("QDRANT_URL");
    //        options.ApiKey = Environment.GetEnvironmentVariable("QDRANT_API_KEY");
    //    });

    //    // OpenAI Settings
    //    services.Configure<OpenAISettings>(options =>
    //    {
    //        options.Url = Environment.GetEnvironmentVariable("OPENAI_URL");
    //        options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    //    });

    //    return services;
    //}

    public static IServiceCollection AddDatabaseContexts(this IServiceCollection services)
    {
        var sqlConnection = Environment.GetEnvironmentVariable("USER_SERVICE_CONNECTION");
        
        services.AddDbContext<UserDbContext>(options => options.UseSqlServer(sqlConnection));

        //var mysqlConnection = Environment.GetEnvironmentVariable("USER_SERVICE_MYSQL_CONNECTION");
        //services.AddDbContext<UserServiceMySqlDbContext>(options => options.UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection)));

        return services;
    }
}
