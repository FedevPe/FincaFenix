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
            services.AddTransient<ITaskQueryService, TaskQueryService>()
                    .AddTransient<IFarmQueryService, FarmQueryService>()
                    .AddTransient<IDetailSectorQueryService, DetailSectorQueryService>()
                    .AddTransient<IMaterialQueryService, MaterialQueryService>()
                    .AddTransient<IWorkOrderQueryService, WorkOrderQueryService>()
                    .AddTransient<IMaterialCategoryQueryService, MaterialCategoryQueryService>()
                    .AddTransient<IMachineQueryService, MachineQueryServices>()
                    .AddTransient<IEmployeeQueryService, EmployeeQueryService>()
                    .AddTransient<IDetailWOQueryService, DetailWOQueryService>();

            //QUERIES
            #region WorkOrder
            services.AddTransient<IGetWorkOrderInformationQuery, GetWorkOrderInformationQuery>();
            #endregion
            //COMMANDS
            #region WorkOrder
            services.AddTransient<ICreateWorkOrderCommand, CreateWorkOrderCommand>();
            services.AddTransient<IUpdateWorkOrderCommand, UpdateWorkOrderCommand>();
            #endregion

            services.AddTransient<IWorkOrderCommandService, WorkOrderCommandService>()
                    .AddTransient<IDetailWOCommandService, DetailWOCommandService>();

            return services;
        }
    }
}
