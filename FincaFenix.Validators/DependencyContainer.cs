using FincaFenix.Validators.Validators.Recipe;
using FincaFenix.Validators.Validators.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddValidatorServices(this IServiceCollection services)
        {
            services.AddScoped<WorkOrderValidator>()
                    .AddScoped<DetailSectorFarmValidator>()
                    .AddScoped<RecipeValidator>()
                    .AddScoped<DetailRecipeValidator>();

            return services;
        }
    }
}
