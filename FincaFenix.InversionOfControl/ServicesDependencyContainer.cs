using AutoMapper;
using FincaFenix.EFCore;
using FincaFenix.UsesCases.Mappings;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependencyContainer
{
    public static IServiceCollection AddServicesContainer(
        this IServiceCollection services,
        IConfiguration configuration = null)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        mapperConfig.AssertConfigurationIsValid();
        services.AddSingleton(mapperConfig.CreateMapper());

        services.AddUseCasesServices()
                .AddControllersServices()
                .AddGatewaysServices()
                .AddViewModelServices()
                .AddEFCoreServices()
                .AddPDFServices();

        if (configuration is not null)
            services.AddAuthServices(configuration);

        return services;
    }
}
