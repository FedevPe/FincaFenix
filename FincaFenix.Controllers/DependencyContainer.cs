using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Controllers.WorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;
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
                .AddTransient<IMaterialCategoryController, MaterialCategoryController>()
                .AddTransient<IMachineController, MachineController>()
                .AddTransient<IEmployeeController, EmployeeController>()

                //WorkOrder
                .AddTransient<ICreateWorkOrderController, CreateWorkOrderController>()
                .AddTransient<IGetWorkOrderInformationController, GetWorkOrderInformationController>()
                .AddTransient<IUpdateWorkOrderController, UpdateWorkOrderController>()

                //DetailWorkOrder
                .AddTransient<IAddDetailWorkOrderController, IAddDetailWorkOrderController>()
                .AddTransient<IGetActivitiesWorkOrderController, IGetActivitiesWorkOrderController>();

        return services;
    }
}
