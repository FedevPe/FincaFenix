using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class CreateWorkOrderRepository (ICreateWorkOrderCommand command) : ICreateWorkOrderRepository
    {
        public async Task<int> CreateWorkOrder(WorkOrderEntity workOrder)
        {
            return await command.AddNewWorkOrder(workOrder);
        }
    }
}
