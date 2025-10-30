namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail
{
    public interface IGetActivitiesWorkOrderInputPort
    {
        Task GetActivitiesByOrderId(int orderId);
    }
}