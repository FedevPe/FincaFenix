using FincaFenix.Gateways.Implementations;
using FincaFenix.UsesCases.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGateways(this IServiceCollection services)
        {
            services.AddTransient<IFarmRepository, FarmRepository>()
                    .AddTransient<IMaterialRepository, MaterialRepository>()
                    .AddTransient<ISectorRepository, SectorRepository>()
                    .AddTransient<ITaskRepository, TaskRepository>()
                    .AddTransient<IWorkOrderRepository, WorkOrderRepository>();

            return services;
        }
    }
}
