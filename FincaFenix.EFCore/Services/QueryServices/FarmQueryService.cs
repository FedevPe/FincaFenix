using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FarmQueryService : FincaFenixContext, IFarmQueryService
    {
        public async Task<IEnumerable<FarmEntity>> GetFarmList()
        {
            return await Farms.OrderBy(t => t.Name).ToListAsync();
        }
    }
}
