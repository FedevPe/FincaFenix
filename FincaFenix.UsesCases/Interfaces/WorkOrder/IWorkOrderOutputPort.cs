using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOutputPort
    {
        public bool IsSaved { get; }
        Task Handle(WorkOrderDTO workOrder);
    }
}
