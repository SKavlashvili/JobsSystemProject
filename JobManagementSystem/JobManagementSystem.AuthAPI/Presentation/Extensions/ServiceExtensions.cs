using FluentValidation;
using JobManagementSystem.AuthAPI.Application;
using JobManagementSystem.AuthAPI.Infrastructure;
using StackExchange.Redis;

namespace JobManagementSystem.AuthAPI.Presentation
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {

            //Adding all validators in DI container
            services.AddValidatorsFromAssemblyContaining<UserRegistrationModel>(ServiceLifetime.Singleton);

            //Custom Services

                //Redis Services
            services.AddSingleton<ConnectionMultiplexer>((IServiceProvider provider) =>
            {
                IConfiguration configuration = provider.GetService<IConfiguration>();
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisMessageQueue"));
            });
            services.AddSingleton<IDatabase>((IServiceProvider provider) =>
            {
                ConnectionMultiplexer conn = provider.GetService<ConnectionMultiplexer>();
                return conn.GetDatabase();
            });
                //Postgre Services
            services.AddSingleton<PostgreSQLConnectionFactory>();

                //Application Layer Services
            services.AddSingleton<IAuthService,AuthService>();


            return services;
        }
    }
}
