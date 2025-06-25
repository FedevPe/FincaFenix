using FincaFenix.Gateways.Implementations;
using FincaFenix.UsesCases.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGatewaysServices(this IServiceCollection services)
        {
            services.AddSingleton<IFarmRepository, FarmRepository>()
                    .AddSingleton<IMaterialRepository, MaterialRepository>()
                    .AddSingleton<IDetailSectorRepository, DetailSectorRepository>()
                    .AddSingleton<ITaskRepository, TaskRepository>()
                    .AddSingleton<IWorkOrderRepository, WorkOrderRepository>();

            return services;
        }
    }
}
