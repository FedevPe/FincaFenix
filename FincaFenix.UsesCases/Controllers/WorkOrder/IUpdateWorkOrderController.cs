namespace FincaFenix.UsesCases.Controllers.WorkOrder
{
    public interface IUpdateWorkOrderController
    {
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
