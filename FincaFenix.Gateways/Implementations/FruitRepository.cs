using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Fruit;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class FruitRepository(
        IFruitQueryService queryService,
        IFruitCommandService commandService) : IFruitRepository
    {
        public async Task<IEnumerable<FruitEntity>> GetAll()
        {
            return await queryService.GetFruitList();
        }

        public async Task<(IEnumerable<FruitEntity> Fruits, int TotalCount)> GetPaged(FruitFilterDTO filter)
        {
            return await queryService.GetFruitListPaged(filter);
        }

        public async Task<FruitEntity> GetById(int id)
        {
            return await queryService.GetFruitById(id);
        }

        public async Task<bool> Exists(int id)
        {
            return await queryService.FruitExists(id);
        }

        public async Task<FruitEntity> Add(FruitEntity fruit)
        {
            return await commandService.CreateAsync(fruit);
        }

        public async Task<FruitEntity> Update(FruitEntity fruit)
        {
            return await commandService.UpdateAsync(fruit);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }
    }
}
