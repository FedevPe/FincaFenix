using FincaFenix.Entities.DTOs.Login;
using FincaFenix.UsesCases.Behaviors;
using FincaFenix.UsesCases.Mappings;
using FincaFenix.UsesCases.UseCases.DetailWorkOrder;
using FincaFenix.UsesCases.UseCases.Inventory;
using FincaFenix.UsesCases.UseCases.Material;
using FincaFenix.UsesCases.UseCases.MaterialCategory;
using FincaFenix.UsesCases.UseCases.Task;
using FincaFenix.UsesCases.UseCases.UnitOfMeasure;
using FincaFenix.UsesCases.UseCases.WorkOrder;
using FincaFenix.Validators.Validators.DetailWorkOrder;
using FincaFenix.Validators.Validators.Inventory;
using FincaFenix.Validators.Validators.Login;
using FincaFenix.Validators.Validators.Material;
using FincaFenix.Validators.Validators.Recipe;
using FincaFenix.Validators.Validators.Task;
using FincaFenix.Validators.Validators.WorkOrder;
using FluentValidation;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<MappingProfile>();
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        })

                //Validators
                .AddTransient<IValidator<LoginDTO>, LoginDTOValidator>()
                .AddTransient<IValidator<CreateWorkOrderCommand>, CreateWorkOrderCommandValidator>()
                .AddTransient<IValidator<UpdateWorkOrderCommand>, UpdateWorkOrderCommandValidator>()
                .AddTransient<IValidator<AddDetailWorkOrderCommand>, AddDetailWorkOrderCommandValidator>()
                .AddTransient<IValidator<RegisterMovementCommand>, RegisterMovementCommandValidator>()
                .AddTransient<IValidator<RegisterConsumptionCommand>, RegisterConsumptionCommandValidator>()
                .AddTransient<IValidator<CreateMaterialCommand>, CreateMaterialCommandValidator>()
                .AddTransient<IValidator<UpdateMaterialCommand>, UpdateMaterialCommandValidator>()
                .AddTransient<IValidator<CreateMaterialCategoryCommand>, CreateMaterialCategoryCommandValidator>()
                .AddTransient<IValidator<UpdateMaterialCategoryCommand>, UpdateMaterialCategoryCommandValidator>()
                .AddTransient<IValidator<CreateUnitOfMeasureCommand>, CreateUnitOfMeasureCommandValidator>()
                .AddTransient<IValidator<UpdateUnitOfMeasureCommand>, UpdateUnitOfMeasureCommandValidator>()
                .AddTransient<IValidator<CreateTaskCommand>, CreateTaskCommandValidator>()
                .AddTransient<IValidator<UpdateTaskCommand>, UpdateTaskCommandValidator>()
                .AddTransient<WorkOrderValidator>()
                .AddTransient<RecipeValidator>()
                .AddTransient<DetailRecipeValidator>()
                .AddTransient<DetailSectorFarmValidator>();

        return services;
    }
}
