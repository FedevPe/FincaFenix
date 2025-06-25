using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Sector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaFenix.EFCore.Services.DetailSector
{
    public class DetailSectorQueryService : FincaFenixContext, IDetailSectorQueryService
    {
        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId)
        {
            return await DetailSectors.Where(x => x.FarmId == farmId).ToListAsync();
        }
    }
}
