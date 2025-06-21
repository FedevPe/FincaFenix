using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMaterialController
    {
        Task<IEnumerable<MaterialDTO>> DisplayListMaterial();
        Task<MaterialDTO> GetMaterialById(int id);
    }
}
