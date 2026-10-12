using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Fruit
{
    public interface IFruitCommandService
    {
        Task<FruitEntity> CreateAsync(FruitEntity fruit);
        Task<FruitEntity> UpdateAsync(FruitEntity fruit);
        Task DeleteAsync(int id);
    }
}
