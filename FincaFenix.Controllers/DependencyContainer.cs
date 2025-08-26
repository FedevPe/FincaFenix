using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Controllers.UpdateWorkOrder;
using FincaFenixControllers.Implementations;
using FincaFenixControllers.Implementations.WorkOrder;

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
                      .AddTransient<IDetailWorkOrderController, DetailWorkOrderController>()
                      .AddTransient<IUpdateWorkOrderController, UpdateWorkOrderController>();

        return services;
    }
}
