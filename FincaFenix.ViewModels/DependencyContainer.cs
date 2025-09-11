using FincaFenix.ViewModels.ViewModels;
using FincaFenix.ViewModels.ViewModels.Login;
using FincaFenix.ViewModels.ViewModels.UpdateWorkOrder;
using FincaFenix.ViewModels.ViewModels.WorkOrder.CreateWorkOrder;
using FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder;
using FincaFenix.ViewModels.ViewModels.WorkOrderDetails;

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
                    .AddTransient<LoadDetailsWorkOrderViewModel>()
                    .AddTransient<UpdateWorkOrderViewModel>()
                    .AddTransient<CalculateMaterialConsumedViewModel>();

            return services;
        }
    }
}
