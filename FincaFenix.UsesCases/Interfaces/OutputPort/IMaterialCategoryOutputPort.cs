using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IMaterialCategoryOutputPort
    {
        public IEnumerable<MaterialCategoryDTO> Categories { get; }
        Task HandleList(IEnumerable<MaterialCategoryEntity> categoriesList);
    }
}
