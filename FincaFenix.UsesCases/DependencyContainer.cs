using FincaFenix.UsesCases.Interactors;
using FincaFenix.UsesCases.Interactors.WorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.Sectors;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        //Interactors
        services.AddTransient<IFarmInputPort, FarmInteractor>()
                .AddTransient<IDetailSectorInputPort, DetailSectorInteractor>()
                .AddTransient<ITaskInputPort, TaskInteractor>()
                .AddTransient<IWorkOrderInputPort, WorkOrderInteractor>()
                .AddTransient<IMaterialInputPort, MaterialInteractor>()
                .AddTransient<IMaterialCategoryInputPort, MaterialCategoryInteractor>()
                .AddTransient<IMachineInputPort, MachineInteractor>()
                .AddTransient<IEmployeeInputPort, EmployeeInteractor>()
                .AddTransient<IAddDetailWorkOrderInputPort, DetailWorkOrderInteractor>()
                .AddTransient<IUpdateWorkOrderInputPort, UpdateWorkOrderInteractor>();


        return services;
    }
}

