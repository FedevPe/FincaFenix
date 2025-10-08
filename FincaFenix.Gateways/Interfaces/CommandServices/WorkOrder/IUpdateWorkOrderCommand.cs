namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IUpdateWorkOrderCommand
    {
        Task<bool> UpdateWorkOrder();
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
