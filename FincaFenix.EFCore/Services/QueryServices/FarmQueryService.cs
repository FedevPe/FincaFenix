using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FarmQueryService(
        FincaFenixContext context) : IFarmQueryService
    {
        public Task<bool> Exists(int id)
        {
            return context.Farms.AnyAsync(f => f.Id == id);
        }

        public Task<FarmEntity> GetFarmById(int farmId)
        {
            return context.Farms
                .Where(f => f.Id == farmId)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Task with ID {farmId} not found.");
        }

        public async Task<IEnumerable<FarmEntity>> GetFarmList()
        {
            return await context.Farms.Where( f => !f.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}
