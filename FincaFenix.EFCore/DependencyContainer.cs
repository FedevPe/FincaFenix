using FincaFenix.EFCore.Services.CommandServices;
using FincaFenix.EFCore.Services.CommandServices.WorkOrder;
using FincaFenix.EFCore.Services.QueryServices;
using FincaFenix.EFCore.Services.QueryServices.WorkOrder;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.Gateways.Interfaces.QueryServices.WorkOrder;

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
                    .AddScoped<IEmployeeQueryService, EmployeeQueryService>()
                    .AddScoped<IDetailWOQueryService, DetailWOQueryService>();

            //QUERIES
            #region WorkOrder
            services.AddScoped<IGetWorkOrderInformationQuery, GetWorkOrderInformationQuery>();
            #endregion
            //COMMANDS
            #region WorkOrder
            services.AddScoped<ICreateWorkOrderCommand, CreateWorkOrderCommand>();
            services.AddScoped<IUpdateWorkOrderCommand, UpdateWorkOrderCommand>();
            #endregion

            services.AddScoped<IWorkOrderCommandService, WorkOrderCommandService>()
                    .AddScoped<IDetailWOCommandService, DetailWOCommandService>();

            return services;
        }
    }
}
