using FincaFenix.UsesCases.Controllers;
using FincaFenixControllers.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        // Controllers
        services.AddTransient<IFarmController, FarmController>()
                      .AddTransient<IDetailSectorController, DetailSectorController>()
                      .AddTransient<IMaterialController, MaterialController>()
                      .AddTransient<ITaskController, TaskController>()
                      .AddTransient<IWorkOrderController, WorkOrderController>()
                      .AddTransient<IMaterialCategoryController, MaterialCategoryController>()
                      .AddTransient<IMachineController, MachineController>()
                      .AddTransient<IEmployeeController, EmployeeController>()
                      .AddTransient<IDetailWorkOrderController, DetailWorkOrderController>();

        return services;
    }
}
