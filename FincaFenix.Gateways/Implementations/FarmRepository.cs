using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class FarmRepository(IFarmQueryService queryService) : IFarmRepository
    {
        public async Task<FarmEntity> GetFarmById(int farmId)
        {
            return await queryService.GetFarmById(farmId);
        }

        public async Task<IEnumerable<FarmEntity>> GetListFarm()
        {
            return await queryService.GetFarmList();
        }
    }
}
