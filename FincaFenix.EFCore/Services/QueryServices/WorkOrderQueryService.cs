using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.Extensions.Options;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class WorkOrderQueryService : FincaFenixContext, IWorkOrderQueryService
    {
        public async Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderNum)
        {
            return await WorkOrders.FindAsync(orderNum);
        }

        public Task<IEnumerable<WorkOrderEntity>> GetWorkOrderListByFarmId(int farmId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WorkOrderEntity>> GetWorkOrderListByTaskId(int taskId)
        {
            throw new NotImplementedException();
        }
    }
}
