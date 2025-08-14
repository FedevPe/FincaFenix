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
            //Services
            services.AddScoped<TextAppBarStateService>()
                    .AddScoped<TotalAreaSectorService>()
                    .AddScoped<PDFService>();

            //Validators
            services.AddScoped<NewOrderDataUIValidator>()
                    .AddScoped<NewRecipeUIValidator>()
                    .AddScoped<CreateWorkOrderUIValidator>()
                    .AddScoped<NewDetailRecipeUIValidator>()
                    .AddScoped<NewDetailSectorUIValidator>()
                    .AddScoped<DetailActivityUIValidator>()
                    .AddScoped<RegistryActivityViewModelValidator>()
                    .AddScoped<RecipeMachineUIValidator>();

            return services;
        }
    }
}
