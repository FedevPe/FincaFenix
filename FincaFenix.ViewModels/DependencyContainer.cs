using FincaFenix.ViewModels.ViewModels;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddViewModelServices(this IServiceCollection services)
        {
            // ViewModels
            services.AddTransient<NewOrderDataViewModel>()
                    .AddTransient<SearchBarViewModel>()
                    .AddTransient<NewRecipeViewModel>()
                    .AddTransient<CreateWorkOrderViewModel>()
                    .AddTransient<GeneralInfoWorkOrderViewModel>()
                    .AddTransient<RegistryActivityViewModel>()
                    .AddTransient<LoadWorkOrdersViewModel>()
                    .AddTransient<LoadDetailsWorkOrderViewModel>();

            return services;
        }
    }
}
