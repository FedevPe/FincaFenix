using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Material
{
    public interface IMaterialOutputProt
    {
        Task HandleList(List<MaterialDTO> materialList);
        Task Handle(MaterialDTO material);
    }
}
