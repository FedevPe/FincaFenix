using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IMaterialOutputPort
    {
        List<MaterialOrderDTO> MaterialList { get; }
        Task HandleList(IEnumerable<MaterialEntity> materialList);
        Task Handle(MaterialEntity material);
    }
}
