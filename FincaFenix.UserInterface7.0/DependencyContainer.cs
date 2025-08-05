using FincaFenix.UserInterface7._0.Services;
using FincaFenix.UserInterface7._0.Validators.DetailWorkOrder;
using FincaFenix.UserInterface7._0.Validators.RegisterActivity;
using FincaFenix.UserInterface7._0.Validators.WorkOrder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddScoped<TextAppBarStateService>()
                    .AddScoped<TotalAreaSectorService>();

            services.AddScoped<NewOrderDataUIValidator>()
                    .AddScoped<NewRecipeUIValidator>()
                    .AddScoped<CreateWorkOrderUIValidator>()
                    .AddScoped<NewDetailRecipeUIValidator>()
                    .AddScoped<NewDetailSectorUIValidator>()
                    .AddScoped<DetailActivityUIValidator>()
                    .AddScoped<RegistryActivityViewModelValidator>();

            return services;
        }
    }
}
