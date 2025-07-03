using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class FarmPresenter : IFarmOutputPort
    {
        public List<FarmDTO>? FarmList { get; private set; }
        public FarmDTO? Farm { get; private set; }

        public Task Handle(FarmEntity task)
        {
            Farm = new FarmDTO
            {
                Id = task.Id,
                Name = task.Name
            };
            return Task.CompletedTask;
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
