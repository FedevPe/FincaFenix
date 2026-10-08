using AutoMapper;
using FincaFenix.EFCore;
using FincaFenix.UsesCases.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependencyContainer
{
    public static IServiceCollection AddServicesContainer(
        this IServiceCollection services,
        IConfiguration configuration = null,
        ILoggerFactory loggerFactory = null)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, loggerFactory ?? NullLoggerFactory.Instance);
        mapperConfig.AssertConfigurationIsValid();
        services.AddSingleton(mapperConfig.CreateMapper());

        services.AddUseCasesServices()
                .AddControllersServices()
                .AddGatewaysServices()
                .AddEFCoreServices()
                .AddPDFServices();

        if (configuration is not null)
            services.AddAuthServices(configuration);

        return services;
    }
}
