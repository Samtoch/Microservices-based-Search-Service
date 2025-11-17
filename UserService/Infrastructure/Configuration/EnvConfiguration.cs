namespace UserService.Infrastructure.Configuration
{

    using DotNetEnv;
    using Microsoft.Extensions.Configuration;

    public static class EnvConfiguration
    {
        public static void LoadEnv(IConfigurationBuilder configurationBuilder)
        {
            // Load .env file
            Env.Load(); //Env.Load(@"C:\Properties\.env");
            configurationBuilder.AddEnvironmentVariables();
        }
    }

}
