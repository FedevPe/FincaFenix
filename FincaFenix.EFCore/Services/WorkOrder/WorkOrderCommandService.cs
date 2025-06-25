using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.WorkOrder;
using System.Net.NetworkInformation;

namespace FincaFenix.EFCore.Services.WorkOrder
{
    public class WorkOrderCommandService : FincaFenixContext, IWorkOrderCommandService
    {
        public async Task<bool> SaveWorkOrder(WorkOrderEntity workOrderEntity)
        {
			try
			{
				await AddAsync(workOrderEntity);
                return await SaveChangesAsync() > 0;
            }
			catch (Exception)
			{

				throw;
			}
        }
    }
}
