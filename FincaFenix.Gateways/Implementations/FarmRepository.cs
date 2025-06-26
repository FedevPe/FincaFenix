using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class FarmRepository(IFarmQueryService queryService) : IFarmRepository
    {
        public Task<FarmEntity> GetFarmById(int farmId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FarmEntity>> GetListFarm()
        {
            return await queryService.GetFarmList();
        }
    }
}
