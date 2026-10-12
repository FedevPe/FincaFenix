using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Farms;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class FarmRepository(
        IFarmQueryService queryService,
        IFarmCommandService commandService) : IFarmRepository
    {
        public Task<bool> Exists(int id)
        {
            return queryService.Exists(id);
        }

        public Task<FarmEntity> GetFarmById(int farmId)
        {
            return queryService.GetFarmById(farmId);
        }

        public async Task<IEnumerable<FarmEntity>> GetListFarm()
        {
            return await queryService.GetFarmList();
        }

        public async Task<(IEnumerable<FarmEntity> Farms, int TotalCount)> GetFarmListPaged(FarmFilterDTO filter)
        {
            return await queryService.GetFarmListPaged(filter);
        }

        public async Task<FarmEntity> Add(FarmEntity farm)
        {
            return await commandService.CreateAsync(farm);
        }

        public async Task<FarmEntity> Update(FarmEntity farm)
        {
            return await commandService.UpdateAsync(farm);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }

        public async Task<FarmEntity> Reactivate(int id)
        {
            return await commandService.ReactivateAsync(id);
        }
    }
}