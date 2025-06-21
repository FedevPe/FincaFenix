using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Material
{
    public interface IMaterialInputPort
    {
        Task GetMaterialList();
        Task GetMaterialById(int id);
    }
}
