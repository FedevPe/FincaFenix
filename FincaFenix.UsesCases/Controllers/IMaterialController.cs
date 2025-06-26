using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialController
    {
        Task<IEnumerable<MaterialOrderDTO>> GetListMaterialByCategoryId(int categoryId);
        Task<IEnumerable<MaterialOrderDTO>> GetListMaterialByRecipeId(int recipeId);
        Task<IEnumerable<MaterialOrderDTO>> GetMaterialList();
    }
}
