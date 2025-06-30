using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace FincaFenix.Presenters.Implementations
{
    public class WorkOrderPresenter : IWorkOrderOutputPort
    {
        public IEnumerable<WorkOrderDTO>? WorkOrderList { get; private set; }
        public bool IsSaved { get; private set; } = false;

        public Task Handle(int workOrderId)
        {
            if (workOrderId != 0)
                IsSaved = true;

            return Task.CompletedTask;
        }

        public Task HandleList(IEnumerable<WorkOrderEntity> workOrdersEntity, IEnumerable<DetailSectorFarmEntity> sectorList)
        {
            //Lógica para covertir WorkOrderEntity a WorkOrderDTO
            return Task.CompletedTask;
        }
    }
}
