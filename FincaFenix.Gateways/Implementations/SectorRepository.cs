using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Sector;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class SectorRepository(ISectorQueryService queryService) : ISectorRepository
    {
        public async Task<IEnumerable<SectorEntity>> GetAllSectorsByFarmId(int farmId)
        {
            return await queryService.GetSectorListByFarmId(farmId);
        }

        public async Task<IEnumerable<SectorEntity>> GetAllSectorsByOrderId(int orderId)
        {
            return await queryService.GetSectorListByFarmId(orderId);
        }
    }
}
