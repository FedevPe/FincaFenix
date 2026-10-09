using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class UnitOfMeasureRepository(
        IUnitOfMeasureQueryService queryService,
        IUnitOfMeasureCommandService commandService) : IUnitOfMeasureRepository
    {
        public async Task<IEnumerable<UnitOfMeasureEntity>> GetList()
        {
            return await queryService.GetList();
        }

        public async Task<UnitOfMeasureEntity> GetById(int id)
        {
            return await queryService.GetById(id);
        }

        public async Task<bool> Exists(int id)
        {
            return await queryService.Exists(id);
        }

        public async Task<UnitOfMeasureEntity> Add(UnitOfMeasureEntity unit)
        {
            return await commandService.CreateAsync(unit);
        }

        public async Task<UnitOfMeasureEntity> Update(UnitOfMeasureEntity unit)
        {
            return await commandService.UpdateAsync(unit);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }
    }
}
