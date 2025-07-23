using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices
{
    public interface IWorkOrderCommandService
    {
        Task<int> SaveWorkOrder(WorkOrderEntity workOrderEntity);
    }
}
