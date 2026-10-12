using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IFruitVarietyRepository
    {
        Task<IEnumerable<FruitVarietyEntity>> GetAll(int? fruitId);
        Task<(IEnumerable<FruitVarietyEntity> Varieties, int TotalCount)> GetPaged(FruitVarietyFilterDTO filter);
        Task<FruitVarietyEntity> GetById(int id);
        Task<bool> Exists(int id);
        Task<FruitVarietyEntity> Add(FruitVarietyEntity variety);
        Task<FruitVarietyEntity> Update(FruitVarietyEntity variety);
        Task Delete(int id);
    }
}
