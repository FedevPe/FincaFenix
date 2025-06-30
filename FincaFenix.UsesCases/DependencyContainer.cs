using FincaFenix.UsesCases.Interactors;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.Sectors;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        //Interactors
        services.AddScoped<IFarmInputPort, FarmInteractor>()
                .AddScoped<ISectorsInputPort, SectorInteractor>()
                .AddScoped<ITaskInputPort, TaskInteractor>()
                .AddScoped<IWorkOrderInputPort, WorkOrderInteractor>()
                .AddScoped<IMaterialInputPort, MaterialInteractor>()
                .AddScoped<IMaterialCategoryInputPort, MaterialCategoryInteractor>()
                .AddScoped<IMachineInputPort, MachineInteractor>()
                .AddScoped<IEmployeeInputPort, EmployeeInteractor>();


        return services;
    }
}

