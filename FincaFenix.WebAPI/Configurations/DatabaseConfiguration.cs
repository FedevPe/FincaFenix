using FincaFenix.EFCore;
using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddIdentityDataBase(
        this IServiceCollection services, IConfiguration configuration)
    {    

        var conn = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<FincaFenixContext>(conf =>
        {
            conf.UseSqlServer(conn);
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
