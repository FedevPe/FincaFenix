using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IWorkOrderQueryService
    {
        Task<IEnumerable<WorkOrderEntity>> GetWorkOrderListByFarmId(int farmId);
        Task<IEnumerable<WorkOrderEntity>> GetWorkOrderListByTaskId(int taskId);
        Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderNum);
    }
}
