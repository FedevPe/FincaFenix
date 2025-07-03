using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace FincaFenix.Presenters.Implementations
{
    public class WorkOrderPresenter : IWorkOrderOutputPort
    {
        public ShowInfoAddActivityFormDTO? WorkOrder { get; private set; }
        public IEnumerable<WorkOrderDTO>? WorkOrderList { get; private set; }
        public IEnumerable<ShowWorkOrderCardDTO>? InfoWorkOrderCard { get; private set; } 
        public int TotalCount { get; private set; }
        public bool IsSaved { get; private set; } = false;

        public Task Handle(ShowInfoAddActivityFormDTO workOrder)
        {
            if (workOrder == null)
                throw new ArgumentNullException(nameof(workOrder), "Work order cannot be null.");
            else
                WorkOrder = workOrder;
            return Task.CompletedTask;
        }
        public Task Handle(int workOrderId)
        {
            if (workOrderId != 0)
                IsSaved = true;

            return Task.CompletedTask;
        }
        public Task HandleList(IEnumerable<ShowWorkOrderCardDTO> listDTO, int totalAcount)
        {
            InfoWorkOrderCard = listDTO;
            TotalCount = totalAcount;
            return Task.CompletedTask;
        }
    }
}
