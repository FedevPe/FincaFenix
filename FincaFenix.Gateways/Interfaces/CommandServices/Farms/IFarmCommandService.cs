using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Farms
{
    public interface IFarmCommandService
    {
        Task<FarmEntity> CreateAsync(FarmEntity farm);
        Task<FarmEntity> UpdateAsync(FarmEntity farm);
        Task DeleteAsync(int id);
        Task<FarmEntity> ReactivateAsync(int id);
    }
}