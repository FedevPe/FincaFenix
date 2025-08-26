namespace FincaFenix.UsesCases.Repository.WorkOrder
{
    public interface IUpdateWorkOrderRepository
    {
        Task<bool> UpdateWorkOrder();
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
