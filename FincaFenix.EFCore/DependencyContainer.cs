using FincaFenix.EFCore.Services;
using FincaFenix.EFCore.Services.CommandServices;
using FincaFenix.EFCore.Services.CommandServices.Employees;
using FincaFenix.EFCore.Services.CommandServices.Farms;
using FincaFenix.EFCore.Services.CommandServices.Fruit;
using FincaFenix.EFCore.Services.CommandServices.Inventory;
using FincaFenix.EFCore.Services.CommandServices.Material;
using FincaFenix.EFCore.Services.CommandServices.Tasks;
using FincaFenix.EFCore.Services.CommandServices.Users;
using FincaFenix.EFCore.Services.CommandServices.WorkOrder;
using FincaFenix.EFCore.Services.QueryServices;
using FincaFenix.EFCore.Services.QueryServices.Inventory;
using FincaFenix.EFCore.Services.QueryServices.WorkOrder;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.CommandServices.Employees;
using FincaFenix.Gateways.Interfaces.CommandServices.Fruit;
using FincaFenix.Gateways.Interfaces.CommandServices.Farms;
using FincaFenix.Gateways.Interfaces.CommandServices.Inventory;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using FincaFenix.Gateways.Interfaces.CommandServices.Tasks;
using FincaFenix.Gateways.Interfaces.CommandServices.Users;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.Gateways.Interfaces.QueryServices.Inventory;
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
                    .AddTransient<IMaterialCategoryQueryService, MaterialCategoryQueryService>()
                    .AddTransient<IUnitOfMeasureQueryService, UnitOfMeasureQueryService>()
                    .AddTransient<IMachineQueryService, MachineQueryServices>()
                    .AddTransient<IEmployeeQueryService, EmployeeQueryService>()
                    .AddTransient<IUserQueryService, UserQueryService>()
                    .AddTransient<IFruitQueryService, FruitQueryService>()
                    .AddTransient<IFruitVarietyQueryService, FruitVarietyQueryService>()
                    .AddTransient<IDetailWOQueryService, DetailWOQueryService>()
                    .AddTransient<IInventoryQueryService, InventoryQueryService>();

            //QUERIES
            #region WorkOrder
            services.AddTransient<IGetWorkOrderInformationQuery, GetWorkOrderInformationQuery>();
            #endregion
            //COMMANDS
            #region WorkOrder
            services.AddTransient<ICorrelativeNumberService, CorrelativeNumberService>();
            services.AddTransient<IWorkOrderCommand, WorkOrderCommand>();
            services.AddTransient<IUpdateWorkOrderCommand, UpdateWorkOrderCommand>();
            #endregion

            services.AddTransient<IRecipeCommand, RecipeCommand>();
            services.AddTransient<IDetailWOCommandService, DetailWOCommandService>();
            services.AddTransient<IInventoryCommandService, InventoryCommandService>();
            services.AddTransient<IWorkOrderInventoryCommand, WorkOrderInventoryCommand>();
            services.AddTransient<IWorkOrderConsumptionCommand, WorkOrderConsumptionCommand>();

            //Materiales / Categorías / Unidades de medida
            services.AddTransient<IMaterialCommandService, MaterialCommandService>();
            services.AddTransient<IMaterialCategoryCommandService, MaterialCategoryCommandService>();
            services.AddTransient<IUnitOfMeasureCommandService, UnitOfMeasureCommandService>();
            services.AddTransient<ITaskCommandService, TaskCommandService>();
            services.AddTransient<IFarmCommandService, FarmCommandService>();
            services.AddTransient<IEmployeeCommandService, EmployeeCommandService>();
            services.AddTransient<IUserCommandService, UserCommandService>();
            services.AddTransient<IFruitCommandService, FruitCommandService>();
            services.AddTransient<IFruitVarietyCommandService, FruitVarietyCommandService>();

            //Unit of Work comparte el DbContext Scoped, por lo que debe registrarse como Scoped
            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();

            return services;
        }
    }
}
