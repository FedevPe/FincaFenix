using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class UpdateWorkOrderCommand : FincaFenixContext, IUpdateWorkOrderCommand
    {
        public Task<bool> UpdateWorkOrderState()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateWorkOrderState(WorkOrderEntity workOrder)
        {
            throw new NotImplementedException();
        }
    }
}
