using FincaFenix.UsesCases.Interactors;
using FincaFenix.UsesCases.Interactors.DetailWorkOrder;
using FincaFenix.UsesCases.Interactors.WorkOrder;
using FincaFenix.UsesCases.Interactors.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.Validators.Validators.Recipe;
using FincaFenix.Validators.Validators.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        //Interactors
        services.AddTransient<IFarmInputPort, FarmInteractor>()
                .AddTransient<IDetailSectorInputPort, DetailSectorInteractor>()
                .AddTransient<ITaskInputPort, TaskInteractor>()
                .AddTransient<IMaterialInputPort, MaterialInteractor>()
                .AddTransient<IMaterialCategoryInputPort, MaterialCategoryInteractor>()
                .AddTransient<IMachineInputPort, MachineInteractor>()
                .AddTransient<IEmployeeInputPort, EmployeeInteractor>()
                .AddTransient<IAddDetailWorkOrderInputPort, DetailWorkOrderInteractor>()
                .AddTransient<IUpdateWorkOrderInputPort, UpdateWorkOrderInteractor>()

                //Work Order Interactors
                .AddTransient<ICreateWorkOrderInputPort, CreateWorkOrderInteractor>()
                .AddTransient<IGetWorkOrderInformationInputPort, GetWorkOrderInformationInteractor>()
                .AddTransient<IUpdateWorkOrderInputPort, UpdateWorkOrderInteractor>()

                //Detail Work Order Interactors
                .AddTransient<IAddDetailWorkOrderInputPort, AddDetailWorkOrderInteractor>()
                .AddTransient<IGetActivitiesWorkOrderInputPort, GetActivitiesWorkOrderInteractor>()

                //Validators
                .AddTransient<WorkOrderValidator>()
                .AddTransient<RecipeValidator>()
                .AddTransient<DetailRecipeValidator>()
                .AddTransient<DetailSectorFarmValidator>();

        return services;
    }
}