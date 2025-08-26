using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.WorkOrder
{
    public interface ICreateWorkOrderRepository
    {
        Task<int> CreateWorkOrder(WorkOrderEntity workOrder);
    }
}
