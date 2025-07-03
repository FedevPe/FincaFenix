using FincaFenix.ViewModels.ViewModels;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddViewModelServices(this IServiceCollection services)
        {
            // ViewModels
            services.AddTransient<InfoNewWorkOrderViewModel>()
                    .AddTransient<SearchBarViewModel>()
                    .AddTransient<NewRecipeViewModel>()
                    .AddTransient<CreateWorkOrderViewModel>()
                    .AddTransient<WorkOrderDataViewModel>()
                    .AddTransient<CreateDetailWorkOrderViewModel>()
                    .AddTransient<LoadWorkOrdersViewModel>();

            return services;
        }
    }
}
