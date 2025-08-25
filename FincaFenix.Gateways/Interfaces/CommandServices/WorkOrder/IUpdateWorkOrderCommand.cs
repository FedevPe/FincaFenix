using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IUpdateWorkOrderCommand
    {
        Task<bool> UpdateWorkOrderState();
        Task<bool> UpdateWorkOrderState(WorkOrderEntity workOrder);
    }
}
