
using ApiGateway.Infrastructures.Extensions;
using ApiGateway.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Env.Load(); // Loads .env file
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            var jwtExpiryHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_HOURS"));
            var loginUsername = Environment.GetEnvironmentVariable("LOGIN_USERNAME");
            var loginPassword = Environment.GetEnvironmentVariable("LOGIN_PASSWORD");

            builder.Services.AddCustomHealthChecks();

            // Add YARP Reverse Proxy
            builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            builder.Services.AddResponseCaching();

            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 2; // 2 requests
                    limiterOptions.Window = TimeSpan.FromSeconds(10);
                    limiterOptions.QueueLimit = 0;
                });
            });

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure Swagger FIRST
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
                    c.SwaggerEndpoint("/api-aggregated/swagger.json", "Aggregated API v1");
                    c.RoutePrefix = "swagger";
                    c.DefaultModelsExpandDepth(-1);
                });
            }

            // Create manual endpoints for health checks that Swagger can discover
            app.MapGet("/health", async (HealthCheckService healthCheckService) =>
            {
                var report = await healthCheckService.CheckHealthAsync();
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                });
                return Results.Content(result, "application/json");
            })
            .WithName("Health")
            .WithDisplayName("Health Check")
            .WithTags("Health")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Overall Health Status",
                Description = "Returns the overall health status of the API gateway"
            });

            app.MapGet("/health/live", async (HealthCheckService healthCheckService) =>
            {
                var report = await healthCheckService.CheckHealthAsync(registration => false);
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                });
                return Results.Content(result, "application/json");
            })
            .WithName("Liveness")
            .WithDisplayName("Liveness Probe")
            .WithTags("Health")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Liveness Check",
                Description = "Indicates whether the service is running"
            });

            app.MapGet("/health/ready", async (HealthCheckService healthCheckService) =>
            {
                var report = await healthCheckService.CheckHealthAsync(registration => registration.Tags.Contains("ready"));
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                });
                return Results.Content(result, "application/json");
            })
            .WithName("Readiness")
            .WithDisplayName("Readiness Probe")
            .WithTags("Health")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Readiness Check",
                Description = "Indicates whether the service is ready to accept requests"
            });

            // Keep the original MapHealthChecks for the health checks UI but don't expose to Swagger
            app.MapHealthChecks("/health/original", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description
                        })
                    });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                }
            }).AllowAnonymous();

            app.MapHealthChecks("/health/live/original", new HealthCheckOptions
            {
                Predicate = _ => false
            }).AllowAnonymous();

            app.MapHealthChecks("/health/ready/original", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            }).AllowAnonymous();

            // Aggregated Swagger endpoint - using different route to avoid conflicts
            app.MapGet("/swagger/aggregated.json", async () =>
            {
                try
                {
                    var httpClient = new HttpClient();

                    // Fetch Swagger JSON from different services
                    var userSwagger = await httpClient.GetStringAsync("http://localhost:5166/swagger/v1/swagger.json");
                    var searchSwagger = await httpClient.GetStringAsync("http://localhost:5050/swagger/v1/swagger.json");
                    var documentsSwagger = await httpClient.GetStringAsync("http://localhost:5050/swagger/v1/swagger.json");

                    var merged = new Dictionary<string, object>
                    {
                        ["openapi"] = "3.0.1",
                        ["info"] = new Dictionary<string, object>
                        {
                            ["title"] = "Aggregated API",
                            ["version"] = "v1"
                        },
                        ["paths"] = new Dictionary<string, object>(),
                        ["components"] = new Dictionary<string, object>()
                    };

                    void MergePaths(string json)
                    {
                        var doc = JsonDocument.Parse(json);
                        foreach (var path in doc.RootElement.GetProperty("paths").EnumerateObject())
                        {
                            var normalizedPath = path.Name.Replace("/api", "");
                            ((Dictionary<string, object>)merged["paths"])[normalizedPath] = JsonSerializer.Deserialize<object>(path.Value.GetRawText());
                        }
                    }

                    MergePaths(userSwagger);
                    MergePaths(searchSwagger);
                    MergePaths(documentsSwagger);

                    return Results.Content(JsonSerializer.Serialize(merged), "application/json");
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error aggregating Swagger: {ex.Message}");
                }
            })
            .WithName("AggregatedSwagger")
            .WithTags("Swagger")
            .WithOpenApi();


            // Login endpoint
            app.MapPost("/login", (UserLogin login) =>
            {
                if (login.Username == loginUsername && login.Password == loginPassword)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(jwtSecret);
                    var startTime = DateTime.UtcNow;
                    var expiryTime = startTime.AddHours(jwtExpiryHours);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.Name, login.Username),
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                        Expires = expiryTime,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Results.Ok(new
                    {
                        token = tokenHandler.WriteToken(token),
                        start = startTime,
                        expires = expiryTime
                    });
                }
                return Results.Unauthorized();
            })
            .WithName("Login")
            .WithTags("Authentication")
            .WithOpenApi();

            app.UseRateLimiter();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();

            // Secure endpoint
            app.MapGet("/secure", [Authorize] () => "This is a secure endpoint")
               .WithName("SecureEndpoint")
               .WithTags("Secure")
               .WithOpenApi();

            // Health Checks UI - map to a specific path to avoid conflicts
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
            });

            // Require JWT for all proxied routes
            app.MapReverseProxy().RequireAuthorization();

            app.Run();
        }
    }
}
