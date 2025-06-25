using FincaFenix.EFCore.Options;
using FincaFenix.EFCore.Services.DetailSector;
using FincaFenix.EFCore.Services.Farm;
using FincaFenix.EFCore.Services.Material;
using FincaFenix.EFCore.Services.Task;
using FincaFenix.EFCore.Services.WorkOrder;
using FincaFenix.Gateways.Interfaces.Farm;
using FincaFenix.Gateways.Interfaces.Material;
using FincaFenix.Gateways.Interfaces.Sector;
using FincaFenix.Gateways.Interfaces.Task;
using FincaFenix.Gateways.Interfaces.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddEFCoreServices(this IServiceCollection services, Action<DBOption> configureDBOptions)
        {
            services.Configure(configureDBOptions);

            services.AddSingleton<ITaskQueryService, TaskQueryService>()
                    .AddSingleton<IFarmQueryService, FarmQueryService>()
                    .AddSingleton<IDetailSectorQueryService, DetailSectorQueryService>()
                    .AddSingleton<IMaterialQueryService, MaterialQueryService>()
                    .AddSingleton<IWorkOrderQueryService, WorkOrderQueryService>();

            services.AddSingleton<IWorkOrderCommandService, WorkOrderCommandService>();

            return services;
        }
    }
}
