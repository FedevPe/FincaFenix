using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IWorkOrderCommand
    {
        Task AddWorkOrder(WorkOrderEntity workOrder);
    }
}
