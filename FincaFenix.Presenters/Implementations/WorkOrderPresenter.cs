using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace FincaFenix.Presenters.Implementations
{
    public class WorkOrderPresenter : IWorkOrderOuputPort
    {
        public bool IsSaved { get; private set; } = false;

        public Task Handle(WorkOrderDTO workOrder)
        {
            if (workOrder.Id != 0)
                IsSaved = true;

            return Task.CompletedTask;
        }
    }
}
