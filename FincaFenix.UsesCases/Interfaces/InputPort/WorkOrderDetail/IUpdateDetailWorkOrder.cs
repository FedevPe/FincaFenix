namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail
{
    public interface IUpdateDetailWorkOrder
    {
        Task UpdateDetailWorkOrder(int orderId, int employeeId, DateTime activityDate);
    }
}