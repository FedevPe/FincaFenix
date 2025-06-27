using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class WorkOrderQueryService : FincaFenixContext, IWorkOrderQueryService
    {
        public async Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderNum)
        {
            return await WorkOrders.FindAsync(orderNum);
        }
        public async Task<string> GetLastNumberTypeDoc(string typeDoc)
        {
           var correlativeNumber = await CorrelativeNumber.FirstOrDefaultAsync(cn => cn.TypeDoc == typeDoc);

            if (correlativeNumber == null)
            {
                correlativeNumber = new CorrelativeNumberEntity { TypeDoc = typeDoc, LastNumber = 0 };
            }

            correlativeNumber.LastNumber += 1;
            await SaveChangesAsync();

            return correlativeNumber.LastNumber.ToString();
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
