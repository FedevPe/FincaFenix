using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;

namespace FincaFenix.UsesCases.Interactors.DetailWorkOrder
{
    public class GetActivitiesWorkOrderInteractor(
        IGetActivitiesWorkOrderOutputPort presenter,
        IGetActivitiesWorkOrderRepository repository) : IGetActivitiesWorkOrderInputPort
    {
        public async Task GetActivitiesByOrderId(int orderId)
        {
            await presenter.HandleList(await repository.GetActivityLogByOrderId(orderId));
        }
    }
}
