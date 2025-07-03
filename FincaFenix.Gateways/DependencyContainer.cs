using FincaFenix.Gateways.Implementations;
using FincaFenix.UsesCases.Repository;

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
                    .AddScoped<IWorkOrderRepository, WorkOrderRepository>()
                    .AddScoped<IMaterialCategoryRepository, MaterialCategoryRepository>()
                    .AddScoped<IMachineRepository, MachineRepository>()
                    .AddScoped<IEmployeeRepository, EmployeeRepository>()
                    .AddScoped<IDetailWorkOrderRepository, DetailWorkOrderRepository>();

            return services;
        }
    }
}
