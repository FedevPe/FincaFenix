using FincaFenix.EFCore;
using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Interceptors;
using FincaFenix.Entities.POCOEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FincaFenix.WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddIdentityDataBase(
        this IServiceCollection services, IConfiguration configuration)
    {    

        var conn = configuration.GetConnectionString("DefaultConnection");

        services.AddSingleton(sp => new SlowQueryLogInterceptor(
            sp.GetRequiredService<ILogger<SlowQueryLogInterceptor>>(),
            configuration.GetValue<int?>("EfCore:SlowQueryThresholdMs") ?? 500));

        services.AddDbContext<FincaFenixContext>((sp, conf) =>
        {
            conf.UseSqlServer(conn);
            conf.AddInterceptors(sp.GetRequiredService<SlowQueryLogInterceptor>());
            PolicyContainer.ConfigureSeeding(conf);
        });
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<FincaFenixContext>();

        return services;
    }
}
