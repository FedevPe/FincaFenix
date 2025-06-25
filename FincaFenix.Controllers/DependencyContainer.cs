using FincaFenix.UsesCases.Controllers;
using FincaFenixControllers.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        // Controllers
        services.AddSingleton<IFarmController, FarmController>()
                .AddSingleton<ISectorController, DetailSectorController>()
                .AddSingleton<IMaterialController, MaterialController>()
                .AddSingleton<ITaskController, TaskController>()
                .AddSingleton<IWorkOrderController, WorkOrderController>();

        return services;
    }
}
