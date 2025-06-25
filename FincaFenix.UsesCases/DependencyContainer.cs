using FincaFenix.UsesCases.Interactors;
using FincaFenix.UsesCases.Interfaces.Farms;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.UsesCases.Interfaces.Tasks;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        //Interactors
        services.AddSingleton<IFarmInputPort, FarmInteractor>();
        services.AddSingleton<ISectorsInputPort, SectorInteractor>();
        services.AddSingleton<ITaskInputPort, TaskInteractor>();
        services.AddSingleton<IWorkOrderInputPort, WorkOrderInteractor>();
        services.AddSingleton<IMaterialInputPort, MaterialInteractor>();

        return services;
    }
}

