using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Material;

namespace FincaFenix.Presenters.Implementations
{
    public class MaterialPresenter : IMaterialOutputPort
    {
        public List<MaterialDTO>? MaterialList { get; private set; }

        public Task Handle(MaterialEntity material)
        {
            throw new NotImplementedException();
        }

        public Task HandleList(IEnumerable<MaterialEntity> materialList)
        {
            MaterialList = materialList.Select(material => new MaterialDTO
            {
                Id = material.Id,
                Name = material.Name
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
