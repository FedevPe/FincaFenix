using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.WorkOrder
{
    public interface IWorkOrderCommandService
    {
        Task<bool> SaveWorkOrder(WorkOrderEntity workOrderEntity);
    }
}
