using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class MaterialCategoryPresenter : IMaterialCategoryOutputPort
    {
        public IEnumerable<MaterialCategoryDTO>? Categories { get; private set; }
        public Task HandleList(IEnumerable<MaterialCategoryEntity> categoriesList)
        {
            Categories = categoriesList.Select(category => new MaterialCategoryDTO
            {
                Id = category.Id,
                Description = category.Description
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
