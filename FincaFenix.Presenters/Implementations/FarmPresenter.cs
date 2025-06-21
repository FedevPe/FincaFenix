using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Farms;

namespace FincaFenix.Presenters.Implementations
{
    public class FarmPresenter : IFarmOutputPort
    {
        public List<FarmDTO>? FarmList { get; private set; }

        public Task Handle(FarmEntity task)
        {
            throw new NotImplementedException();
        }

        public Task HandleList(IEnumerable<FarmEntity> materialList)
        {
            FarmList = materialList.Select(farm => new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
