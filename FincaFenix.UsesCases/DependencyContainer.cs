using FincaFenix.Entities.DTOs.Login;
using FincaFenix.UsesCases.Behaviors;
using FincaFenix.UsesCases.Mappings;
using FincaFenix.UsesCases.UseCases.DetailWorkOrder;
using FincaFenix.UsesCases.UseCases.WorkOrder;
using FincaFenix.Validators.Validators.DetailWorkOrder;
using FincaFenix.Validators.Validators.Login;
using FincaFenix.Validators.Validators.Recipe;
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
                .AddTransient<IValidator<AddDetailWorkOrderCommand>, AddDetailWorkOrderCommandValidator>()
                .AddTransient<WorkOrderValidator>()
                .AddTransient<RecipeValidator>()
                .AddTransient<DetailRecipeValidator>()
                .AddTransient<DetailSectorFarmValidator>();

        return services;
    }
}
