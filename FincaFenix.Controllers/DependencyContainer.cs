using FincaFenix.UsesCases.Controllers;
using FincaFenixControllers.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        // Controllers
        services.AddScoped<IFarmController, FarmController>()
                .AddScoped<ISectorController, SectorController>()
                .AddScoped<IMaterialController, MaterialController>()
                .AddScoped<ITaskController, TaskController>()
                .AddScoped<IWorkOrderController, WorkOrderController>();

        return services;
    }
}
