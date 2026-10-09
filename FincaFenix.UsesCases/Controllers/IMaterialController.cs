using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialController
    {
        Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByCategoryId(int categoryId);
        Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByRecipeId(int recipeId);
        Task<IEnumerable<MaterialRecipeDTO>> GetMaterialList();
        Task<MaterialDTO> GetMaterialById(int id);
        Task<PagedResult<MaterialDTO>> GetMaterialListPaged(MaterialFilterDTO filter);
        Task<MaterialDTO> CreateMaterial(CreateMaterialDTO dto);
        Task<MaterialDTO> UpdateMaterial(UpdateMaterialDTO dto);
        Task<bool> DeleteMaterial(int id);
    }
}
