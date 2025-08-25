using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.WorkOrder
{
    public interface IUpdateWorkOrderRepository
    {
        Task<bool> UpdateWorkOrderState();
        Task<bool> UpdateWorkOrderState(WorkOrderEntity workOrder);
    }
}
