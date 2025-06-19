using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderInputPort
    {
        Task Handle(WorkOrderDTO workOrder);
    }
}
