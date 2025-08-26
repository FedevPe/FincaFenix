using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.UsesCases.Interactors.WorkOrder
{
    public class UpdateWorkOrderInteractor(
        IUpdateWorkOrderOutputPort presenter,
        IUpdateWorkOrderRepository repository) : IUpdateWorkOrderInputPort
    {
        public Task UpdateWorkOrder()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            await presenter.Handle(await repository.UpdateWorkOrderState(workOrderId, newStatus));
        }
    }
}
