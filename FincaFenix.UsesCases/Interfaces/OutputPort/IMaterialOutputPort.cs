using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IMaterialOutputPort
    {
        List<MaterialRecipeDTO> MaterialList { get; }
        Task HandleList(IEnumerable<MaterialEntity> materialList);
        Task Handle(MaterialEntity material);
    }
}
