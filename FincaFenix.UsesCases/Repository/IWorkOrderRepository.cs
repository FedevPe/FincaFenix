using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IWorkOrderRepository
    {
        Task<bool> AddWorkOrder(WorkOrderEntity workOrder);
        Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderNum);
        Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByFarmId(int farmId);
        Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByTaskId(int taskId);
    }
}
