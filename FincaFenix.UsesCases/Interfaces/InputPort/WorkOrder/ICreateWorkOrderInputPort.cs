using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder
{
    public interface ICreateWorkOrderInputPort
    {
        Task CreateWorkOrder(WorkOrderDTO workOrder);
    }
}
