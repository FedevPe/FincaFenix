using FincaFenix.EFCore.Context;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class UpdateWorkOrderCommand(
        FincaFenixContext context) : IUpdateWorkOrderCommand
    {
        public Task<bool> UpdateWorkOrder()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            var workOrder = await context.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == workOrderId);

            if (workOrder == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(newStatus))
            {
                workOrder.Status = newStatus;

                if(newStatus == "Cerrado")
                {
                    workOrder.EndDate = DateTime.Now;
                }
            }

            int changesSaved = await context.SaveChangesAsync();
            return changesSaved > 0;
        }
    }
}
