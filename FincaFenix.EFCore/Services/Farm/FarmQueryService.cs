using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Farm;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaFenix.EFCore.Services.Farm
{
    public class FarmQueryService: FincaFenixContext, IFarmQueryService
    {
        public async Task<IEnumerable<FarmEntity>> GetFarmList()
        {
            return await Farms.ToListAsync();
        }
    }
}
