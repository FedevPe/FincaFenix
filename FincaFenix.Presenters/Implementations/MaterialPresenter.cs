using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class MaterialPresenter : IMaterialOutputPort
    {

        public List<MaterialOrderDTO>? MaterialList { get; private set; }

        public Task Handle(MaterialEntity material)
        {
            throw new NotImplementedException();
        }

        public Task HandleList(IEnumerable<MaterialEntity> materialList)
        {
            MaterialList = materialList.Select(material => new MaterialOrderDTO
            {
                Id = material.Id,
                ArticleName = material.ArticleName,
                CommercialName = material.CommercialName,
                CategoryId = material.CategoryId,
                Brand = material.Brand

            }).ToList();

            return Task.CompletedTask;
        }
    }
}
