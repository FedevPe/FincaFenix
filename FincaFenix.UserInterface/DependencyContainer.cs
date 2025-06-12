using FincaFenix.UserInterface.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddUserInterfaceServices(this IServiceCollection services)
        {
            services.AddSingleton<TextAppBarState>();

            return services;
        }
    }
}
