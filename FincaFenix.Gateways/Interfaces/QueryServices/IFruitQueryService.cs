using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IFruitQueryService
    {
        Task<IEnumerable<FruitEntity>> GetFruitList();
        Task<(IEnumerable<FruitEntity> Fruits, int TotalCount)> GetFruitListPaged(FruitFilterDTO filter);
        Task<FruitEntity> GetFruitById(int id);
        Task<bool> FruitExists(int id);
    }
}
