using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FruitDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IFruitVarietyController
    {
        Task<IEnumerable<FruitVarietyDTO>> GetVarieties(int? fruitId);
        Task<PagedResult<FruitVarietyDTO>> GetVarietiesPaged(FruitVarietyFilterDTO filter);
        Task<FruitVarietyDTO> GetVarietyById(int id);
        Task<FruitVarietyDTO> CreateVariety(SaveFruitVarietyDTO dto);
        Task<FruitVarietyDTO> UpdateVariety(SaveFruitVarietyDTO dto);
        Task<bool> DeleteVariety(int id);
    }
}
