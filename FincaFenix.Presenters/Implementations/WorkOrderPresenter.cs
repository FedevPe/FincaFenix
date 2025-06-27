using FincaFenix.UsesCases.Interfaces.WorkOrder;

namespace FincaFenix.Presenters.Implementations
{
    public class WorkOrderPresenter : IWorkOrderOutputPort
    {
        public bool IsSaved { get; private set; } = false;

        public Task Handle(int workOrderId)
        {
            if (workOrderId != 0)
                IsSaved = true;

            return Task.CompletedTask;
        }
    }
}
