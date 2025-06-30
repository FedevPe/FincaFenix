using FincaFenix.UsesCases.Controllers;
using FincaFenixControllers.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        // Controllers
        services.AddTransient<IFarmController, FarmController>()
                      .AddTransient<ISectorController, DetailSectorController>()
                      .AddTransient<IMaterialController, MaterialController>()
                      .AddTransient<ITaskController, TaskController>()
                      .AddTransient<IWorkOrderController, WorkOrderController>()
                      .AddTransient<IMaterialCategoryController, MaterialCategoryController>()
                      .AddTransient<IMachineController, MachineController>()
                      .AddTransient<IEmployeeController, EmployeeController>();

        return services;
    }
}
