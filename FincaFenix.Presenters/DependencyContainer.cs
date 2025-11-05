using FincaFenix.Presenters.Implementations;
using FincaFenix.Presenters.Implementations.DetailWorkOrder;
using FincaFenix.Presenters.Implementations.WorkOrder;
using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.UsesCases.Interfaces.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddPresenterServices(this IServiceCollection services)
    {
        services.AddScoped<IFarmOutputPort, FarmPresenter>()
                .AddScoped<IMaterialOutputPort, MaterialPresenter>()
                .AddScoped<IDetailSectorOutputPort, SectorPresenter>()
                .AddScoped<ITaskOutputPort, TaskPresenter>()
                .AddScoped<IMaterialCategoryOutputPort, MaterialCategoryPresenter>()
                .AddScoped<IMachineOutputPort, MachinePresenter>()
                .AddScoped<IEmployeeOutputPort, EmployeePresenter>()
                .AddScoped<IDetailWorkOrderOutputPort, DetailWorkOrderPresenter>()

                //Work Order Presenter
                .AddScoped<ICreateWorkOrderOutputPort, CreateWorkOrderPresenter>()
                .AddScoped<IGetWorkOrderInformationOutputPort, GetWorkOrderInformationPresenter>()
                .AddScoped<IUpdateWorkOrderOutputPort, UpdateWorkOrderPresenter>()

                //Detail Work Order Presenter
                .AddScoped<IAddDetailWorkOrderOutputPort, AddDetailWorkOrderPresenter>()
                .AddScoped<IGetActivitiesWorkOrderOutputPort, GetActivitiesWorkOrderPresenter>();

        return services;
    }
}