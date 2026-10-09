using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialCategoryController
    {
        Task<IEnumerable<MaterialCategoryDTO>> GetAllCategories();
        Task<MaterialCategoryDTO> GetCategoryById(int id);
        Task<MaterialCategoryDTO> CreateCategory(SaveMaterialCategoryDTO dto);
        Task<MaterialCategoryDTO> UpdateCategory(SaveMaterialCategoryDTO dto);
        Task<bool> DeleteCategory(int id);
    }
}
