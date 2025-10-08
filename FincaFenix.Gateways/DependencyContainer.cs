using FincaFenix.Gateways.Implementations;
using FincaFenix.Gateways.Implementations.WorkOrder;
using FincaFenix.UsesCases.Repository;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGatewaysServices(this IServiceCollection services)
        {
            services.AddScoped<IFarmRepository, FarmRepository>()
                    .AddScoped<IMaterialRepository, MaterialRepository>()
                    .AddScoped<IDetailSectorRepository, DetailSectorRepository>()
                    .AddScoped<ITaskRepository, TaskRepository>()
                    .AddScoped<IMaterialCategoryRepository, MaterialCategoryRepository>()
                    .AddScoped<IMachineRepository, MachineRepository>()
                    .AddScoped<IEmployeeRepository, EmployeeRepository>()
                    .AddScoped<IDetailWorkOrderRepository, DetailWorkOrderRepository>()



                    //Work Order Repository
                    .AddScoped<ICreateWorkOrderRepository, CreateWorkOrderRepository>()
                    .AddScoped<IGetWorkOrderInformationRepository, GetWorkOrderInformationRepository>()
                    .AddScoped<IUpdateWorkOrderRepository, UpdateWorkOrderRepositor>();

            return services;
        }
    }
}
