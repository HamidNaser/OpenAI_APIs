using Dapper;

namespace OpenAI_APIs
{
    public static class Extensions
    {

        public static IServiceCollection AddConnectServicesAndRepositories(this IServiceCollection services, IServiceRegistration registrationProvider = null)
        {
            registrationProvider ??= new ServiceRegistration();
            registrationProvider.RegisterAll(services);
            return services;
        }
    }
}
