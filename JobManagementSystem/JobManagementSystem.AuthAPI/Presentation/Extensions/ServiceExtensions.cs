using FluentValidation;
using JobManagementSystem.AuthAPI.Application;
using JobManagementSystem.AuthAPI.Infrastructure;

namespace JobManagementSystem.AuthAPI.Presentation
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {

            //Adding all validators in DI container
            services.AddValidatorsFromAssemblyContaining<UserRegistrationModel>(ServiceLifetime.Singleton);

            //Custom Services
            services.AddSingleton<PostgreSQLConnectionFactory>();
            services.AddSingleton<IAuthService,AuthService>();


            return services;
        }
    }
}
