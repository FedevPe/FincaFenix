using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Fruit
{
    public interface IFruitVarietyCommandService
    {
        Task<FruitVarietyEntity> CreateAsync(FruitVarietyEntity variety);
        Task<FruitVarietyEntity> UpdateAsync(FruitVarietyEntity variety);
        Task DeleteAsync(int id);
    }
}
