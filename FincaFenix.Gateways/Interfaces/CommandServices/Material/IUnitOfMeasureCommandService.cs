using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Material
{
    public interface IUnitOfMeasureCommandService
    {
        Task<UnitOfMeasureEntity> CreateAsync(UnitOfMeasureEntity unit);
        Task<UnitOfMeasureEntity> UpdateAsync(UnitOfMeasureEntity unit);
        Task DeleteAsync(int id);
    }
}
