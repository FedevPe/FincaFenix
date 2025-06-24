using FincaFenix.EFCore.Options;
using Microsoft.Extensions.DependencyInjection;

namespace FincaFenix.InversionOfControl
{
    public static class ServicesDependencyContainer
    {
        public static IServiceCollection AddDBContextServices(
            this IServiceCollection services,
            Action<DBOption> configureDBOptions)
        {
            services.AddUseCasesServices()
                    .AddGatewaysServices()
                    .AddControllersServices()
                    .AddPresenterServices()
                    .AddEFCoreServices(configureDBOptions);

            return services;
        }
    }
}
