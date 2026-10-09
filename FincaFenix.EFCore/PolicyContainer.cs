using FincaFenix.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FincaFenix.EFCore
{
    public static class PolicyContainer
    {
        public static void ConfigureSeeding(DbContextOptionsBuilder options)
        {
            options.UseAsyncSeeding(async (context, _, ct) =>
            {
                var dbContext = (FincaFenixContext)context;
                var logger = context.GetService<ILogger<FincaFenixContext>>();
                await SeedDataBase.SeedAddPoliciesAsync(dbContext, logger, ct);
                await SeedDataBase.SeedInventoryUnitsAsync(dbContext, logger, ct);
            });
        }
    }
}