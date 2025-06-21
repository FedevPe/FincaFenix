using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Material
{
    public interface IMaterialOutputPort
    {
        List<MaterialDTO> MaterialList { get; }
        Task HandleList(IEnumerable<MaterialEntity> materialList);
        Task Handle(MaterialEntity material);
    }
}
