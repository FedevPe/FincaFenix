
using FincaFenix.UserInterface7._0.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            services.AddScoped<TextAppBarState>()
                    .AddScoped<TotalAreaSectorService>();

            return services;
        }
    }
}
