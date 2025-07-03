using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class DetailSectorQueryService : FincaFenixContext, IDetailSectorQueryService
    {
        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId)
        {
            return await DetailSectors.Where(x => x.FarmId == farmId)
                .OrderBy(x => x.SectorName)
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
