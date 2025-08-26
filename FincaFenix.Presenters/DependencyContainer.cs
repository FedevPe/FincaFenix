using FincaFenix.Presenters.Implementations;
using FincaFenix.Presenters.Implementations.WorkOrder;
using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
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
                .AddScoped<IDetailSectorOutputPort, SectorPresenter>()
                .AddScoped<ITaskOutputPort, TaskPresenter>()
                .AddScoped<IWorkOrderOutputPort, WorkOrderPresenter>()
                .AddScoped<IMaterialCategoryOutputPort, MaterialCategoryPresenter>()
                .AddScoped<IMachineOutputPort, MachinePresenter>()
                .AddScoped<IEmployeeOutputPort, EmployeePresenter>()
                .AddScoped<IDetailWorkOrderOutputPort, DetailWorkOrderPresenter>()
                .AddScoped<IUpdateWorkOrderOutputPort, UpdateWorkOrderPresenter>();

        return services;
    }
}
