namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder
{
    public interface IUpdateWorkOrderInputPort
    {
        Task UpdateWorkOrderState(int workOrderId, string newStatus);
        Task UpdateWorkOrder();
    }
}
