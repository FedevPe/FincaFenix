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
        services.AddScoped<IFarmInputPort, FarmInteractor>();
        services.AddScoped<ISectorsInputPort, SectorInteractor>();
        services.AddScoped<ITaskInputPort, TaskInteractor>();
        services.AddScoped<IWorkOrderInputPort, WorkOrderInteractor>();
        services.AddScoped<IMaterialInputPort, MaterialInteractor>();

        return services;
    }
}

