using FincaFenix.EFCore.Options;
using FincaFenix.EFCore.Services.Task;
using FincaFenix.Gateways.Interfaces.Farm;
using FincaFenix.Gateways.Interfaces.Task;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddEFCoreServices(this IServiceCollection services, Action<DBOption> configureDBOptions)
        {
            services.Configure(configureDBOptions);

            services.AddTransient<ITaskQueryService, TaskQueryService>();

            return services;
        }
    }
}
