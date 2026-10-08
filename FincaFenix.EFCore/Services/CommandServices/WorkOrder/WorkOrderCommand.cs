using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class WorkOrderCommand(FincaFenixContext context) : IWorkOrderCommand
    {
        public Task AddWorkOrder(WorkOrderEntity workOrder)
        {
            context.WorkOrders.Add(workOrder);
            return Task.CompletedTask;
        }
    }
}
