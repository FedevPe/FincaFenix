using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class DetailSectorQueryService : FincaFenixContext, IDetailSectorQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await DetailSectors.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId)
        {
            return await DetailSectors.Where(ds => ds.FarmId == farmId)
                .Include(ds => ds.Variety)
                .ThenInclude(v => v.Fruit)
                .OrderBy(ds => ds.Variety.Fruit.Description)
                .ToListAsync();
        }

        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByOrderId(int orderId)
        {
            return await WorkOrderWorkedSectors.Where(x => x.WorkOrderId == orderId)
                .Select(x => x.SectorFarm)
                .OrderBy(x => x.SectorName)
                .ToListAsync();
        }
    }
}
