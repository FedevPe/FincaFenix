using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Material
{
    public interface IMaterialInputPort
    {
        Task HandleList(List<MaterialDTO> material);
        Task Handle(MaterialDTO material);
        Task GetMaterial(int idSector);
        Task GetListMaterial();
        Task GetListMaterialByOrderId(int idOrder);
    }
}
