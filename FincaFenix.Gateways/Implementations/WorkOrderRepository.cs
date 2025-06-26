using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class WorkOrderRepository(
        IWorkOrderCommandService commandService,
        IWorkOrderQueryService queryService) : IWorkOrderRepository
    {
        public async Task<bool> AddWorkOrder(WorkOrderEntity workOrder)
        {
            return await commandService.SaveWorkOrder(workOrder);
        }

        public async Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByFarmId(int farmId)
        {
            return await queryService.GetWorkOrderListByFarmId(farmId);
        }

        public async Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByTaskId(int taskId)
        {
            return await queryService.GetWorkOrderListByTaskId(taskId);
        }

        public Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderId)
        {
            return queryService.GetWorkOrderByOrderNum(orderId);
        }
    }
}
