using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class DetailSectorRepository(IDetailSectorQueryService queryService) : IDetailSectorRepository
    {
        public async Task<IEnumerable<DetailSectorFarmEntity>> GetAllSectorsByFarmId(int farmId)
        {
            return await queryService.GetSectorListByFarmId(farmId);
        }

        public async Task<IEnumerable<DetailSectorFarmEntity>> GetAllSectorsByOrderId(int orderId)
        {
            return await queryService.GetSectorListByFarmId(orderId);
        }
    }
}
