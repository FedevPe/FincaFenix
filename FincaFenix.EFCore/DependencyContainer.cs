using FincaFenix.EFCore.Services.CommandServices;
using FincaFenix.EFCore.Services.QueryServices;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.QueryServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddEFCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskQueryService, TaskQueryService>()
                    .AddScoped<IFarmQueryService, FarmQueryService>()
                    .AddScoped<IDetailSectorQueryService, DetailSectorQueryService>()
                    .AddScoped<IMaterialQueryService, MaterialQueryService>()
                    .AddScoped<IWorkOrderQueryService, WorkOrderQueryService>()
                    .AddScoped<IMaterialCategoryQueryService, MaterialCategoryQueryService>()
                    .AddScoped<IMachineQueryService, MachineQueryServices>()
                    .AddScoped<IEmployeeQueryService, EmployeeQueryService>();

            services.AddScoped<IWorkOrderCommandService, WorkOrderCommandService>();

            return services;
        }
    }
}
