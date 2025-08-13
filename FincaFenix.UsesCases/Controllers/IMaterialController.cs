using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialController
    {
        Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByCategoryId(int categoryId);
        Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByRecipeId(int recipeId);
        Task<IEnumerable<MaterialRecipeDTO>> GetMaterialList();
    }
}
