using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace ApiGateway.Infrastructures.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            var dbConnection = Environment.GetEnvironmentVariable("USER_SERVICE_CONNECTION");
            var qdrantUrl = Environment.GetEnvironmentVariable("QDRANT_URL");
            var openAiUrl = Environment.GetEnvironmentVariable("OPENAI_URL");

            services.AddHealthChecks()
                .AddSqlServer(dbConnection, name: "SQL Server")
                .AddUrlGroup(new Uri(qdrantUrl), name: "Qdrant API")
                .AddUrlGroup(new Uri(openAiUrl), name: "OpenAI API");

            return services;
        }
    }
}


