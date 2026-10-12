using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FruitDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IFruitController
    {
        Task<IEnumerable<FruitDTO>> GetFruits();
        Task<PagedResult<FruitDTO>> GetFruitsPaged(FruitFilterDTO filter);
        Task<FruitDTO> GetFruitById(int id);
        Task<FruitDTO> CreateFruit(SaveFruitDTO dto);
        Task<FruitDTO> UpdateFruit(SaveFruitDTO dto);
        Task<bool> DeleteFruit(int id);
    }
}
