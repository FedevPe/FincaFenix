using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialCategoryController
    {
        Task<IEnumerable<MaterialCategoryDTO>> GetAllCategories();
    }
}
