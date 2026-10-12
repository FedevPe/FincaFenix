using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Fruit;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class FruitVarietyRepository(
        IFruitVarietyQueryService queryService,
        IFruitVarietyCommandService commandService) : IFruitVarietyRepository
    {
        public async Task<IEnumerable<FruitVarietyEntity>> GetAll(int? fruitId)
        {
            return await queryService.GetVarietyList(fruitId);
        }

        public async Task<(IEnumerable<FruitVarietyEntity> Varieties, int TotalCount)> GetPaged(FruitVarietyFilterDTO filter)
        {
            return await queryService.GetVarietyListPaged(filter);
        }

        public async Task<FruitVarietyEntity> GetById(int id)
        {
            return await queryService.GetVarietyById(id);
        }

        public async Task<bool> Exists(int id)
        {
            return await queryService.VarietyExists(id);
        }

        public async Task<FruitVarietyEntity> Add(FruitVarietyEntity variety)
        {
            return await commandService.CreateAsync(variety);
        }

        public async Task<FruitVarietyEntity> Update(FruitVarietyEntity variety)
        {
            return await commandService.UpdateAsync(variety);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }
    }
}
