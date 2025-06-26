using FincaFenix.ViewModels.ViewModels;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddViewModelServices(this IServiceCollection services)
        {
            // ViewModels
            services.AddTransient<InfoNewWorkOrderViewModel>();
            services.AddTransient<SearchBarViewModel>();
            services.AddTransient<NewRecipeViewModel>();

            return services;
        }
    }
}
