using Microsoft.Extensions.DependencyInjection;

namespace FincaFenix.InversionOfControl
{
    public static class ServicesDependencyContainer
    {
        public static IServiceCollection AddServicesContainer(
            this IServiceCollection services)
        {
            services.AddUseCasesServices()
                    .AddControllersServices()
                    .AddGatewaysServices()
                    .AddPresenterServices()
                    .AddViewModelServices()
                    .AddEFCoreServices();

            return services;
        }
    }
}
