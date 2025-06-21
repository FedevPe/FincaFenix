using FincaFenix.Presenters.Implementations;
using FincaFenix.UsesCases.Interfaces.Farms;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.UsesCases.Interfaces.Tasks;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddPresenterServices(this IServiceCollection services)
    {
        services.AddScoped<IFarmOutputPort, FarmPresenter>()
                .AddScoped<IMaterialOutputPort, MaterialPresenter>()
                .AddScoped<ISectorOutputPort, SectorPresenter>()
                .AddScoped<ITaskOuputPort, TaskPresenter>()
                .AddScoped<IWorkOrderOuputPort, WorkOrderPresenter>();

        return services;
    }
}
