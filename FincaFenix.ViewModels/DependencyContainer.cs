using FincaFenix.ViewModels.ViewModels;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddViewModelServices(this IServiceCollection services)
        {
            // ViewModels
            services.AddScoped<InfoCreateOrderViewModel>();

            return services;
        }
    }
}
