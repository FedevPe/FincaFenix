using FincaFenix.EFCore.Options;
using Microsoft.Extensions.DependencyInjection;

namespace FincaFenix.InversionOfControl
{
    public static class ServicesDependencyContainer
    {
        public static IServiceCollection AddServicesContainer(
            this IServiceCollection services,
            Action<DBOption> configureDBOptions)
        {
            services.AddUseCasesServices()
                    .AddControllersServices()
                    .AddGatewaysServices()
                    .AddPresenterServices()
                    .AddViewModelServices()
                    .AddEFCoreServices(configureDBOptions);

            return services;
        }
    }
}
