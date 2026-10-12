using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IFruitVarietyQueryService
    {
        Task<IEnumerable<FruitVarietyEntity>> GetVarietyList(int? fruitId);
        Task<(IEnumerable<FruitVarietyEntity> Varieties, int TotalCount)> GetVarietyListPaged(FruitVarietyFilterDTO filter);
        Task<FruitVarietyEntity> GetVarietyById(int id);
        Task<bool> VarietyExists(int id);
    }
}
