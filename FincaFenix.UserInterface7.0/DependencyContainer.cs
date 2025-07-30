using FincaFenix.UserInterface7._0.Services;
using FincaFenix.UserInterface7._0.Validators;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddScoped<TextAppBarState>()
                    .AddScoped<TotalAreaSectorService>();

            services.AddScoped<NewOrderDataValidator>()
                    .AddScoped<NewRecipeValidator>()
                    .AddScoped<CreateWorkOrderValidator>();

            return services;
        }
    }
}
