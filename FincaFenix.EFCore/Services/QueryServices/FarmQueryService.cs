using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FarmQueryService : FincaFenixContext, IFarmQueryService
    {
        public Task<FarmEntity> GetFarmById(int farmId)
        {
            return Farms
                .Where(f => f.Id == farmId)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Task with ID {farmId} not found.");
        }

        public async Task<IEnumerable<FarmEntity>> GetFarmList()
        {
            return await Farms.OrderBy(t => t.Name).ToListAsync();
        }
    }
}
