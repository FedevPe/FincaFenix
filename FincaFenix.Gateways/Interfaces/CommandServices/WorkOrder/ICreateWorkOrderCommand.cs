using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface ICreateWorkOrderCommand
    {
        Task<int> AddNewWorkOrder(WorkOrderEntity workOrderEntity);
    }
}
