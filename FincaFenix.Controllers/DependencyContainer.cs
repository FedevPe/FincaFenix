using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Controllers.Inventory;
using FincaFenix.UsesCases.Controllers.WorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;
using FincaFenixControllers.Implementations;
using FincaFenixControllers.Implementations.DetailWorkOrder;
using FincaFenixControllers.Implementations.Inventory;
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
                .AddTransient<IUnitOfMeasureController, UnitOfMeasureController>()
                .AddTransient<IMachineController, MachineController>()
                .AddTransient<IEmployeeController, EmployeeController>()
                .AddTransient<IInventoryController, InventoryController>()

                //WorkOrder
                .AddTransient<ICreateWorkOrderController, WorkOrderController>()
                .AddTransient<IGetWorkOrderInformationController, WorkOrderController>()
                .AddTransient<IUpdateWorkOrderController, WorkOrderController>()

                //DetailWorkOrder
                .AddTransient<IAddDetailWorkOrderController, DetailWorkOrderController>()
                .AddTransient<IGetActivitiesWorkOrderController, DetailWorkOrderController>();

        return services;
    }
}
