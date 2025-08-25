using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class UpdateWorkOrderRepositor(
        IUpdateWorkOrderCommand command) : IUpdateWorkOrderRepository
    {
        public Task<bool> UpdateWorkOrderState()
        {
            return command.UpdateWorkOrderState();
        }

        public Task<bool> UpdateWorkOrderState(WorkOrderEntity workOrder)
        {
            return command.UpdateWorkOrderState(workOrder);
        }
    }
}
