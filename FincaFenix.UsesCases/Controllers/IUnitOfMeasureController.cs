using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IUnitOfMeasureController
    {
        Task<IEnumerable<UnitOfMeasureDTO>> GetUnitOfMeasureList();
        Task<UnitOfMeasureDTO> GetUnitOfMeasureById(int id);
        Task<UnitOfMeasureDTO> CreateUnitOfMeasure(SaveUnitOfMeasureDTO dto);
        Task<UnitOfMeasureDTO> UpdateUnitOfMeasure(SaveUnitOfMeasureDTO dto);
        Task<bool> DeleteUnitOfMeasure(int id);
    }
}
