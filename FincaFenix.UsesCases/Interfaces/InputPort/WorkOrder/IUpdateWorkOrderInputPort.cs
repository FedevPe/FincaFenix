namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder
{
    public interface IUpdateWorkOrderInputPort
    {
        Task UpdateWorkOrderState();
        Task UpdateWorkOrder();
    }
}
