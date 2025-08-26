using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class UpdateWorkOrderRepositor(
        IUpdateWorkOrderCommand command) : IUpdateWorkOrderRepository
    {
        public Task<bool> UpdateWorkOrder()
        {
            return command.UpdateWorkOrder();
        }

        public Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            return command.UpdateWorkOrderState(workOrderId, newStatus);
        }
    }
}
