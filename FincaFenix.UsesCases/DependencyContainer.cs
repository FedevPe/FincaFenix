using FincaFenix.UsesCases.Interactors;
using FincaFenix.UsesCases.Interfaces.Farms;
using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.UsesCases.Interfaces.Tasks;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        //Interactors
        services.AddTransient<IFarmInputPort, FarmInteractor>();
        services.AddTransient<ISectorsInputPort, SectorInteractor>();
        services.AddTransient<ITaskInputPort, TaskInteractor>();
        services.AddTransient<IWorkOrderInputPort, WorkOrderInteractor>();

        return services;
    }
}

