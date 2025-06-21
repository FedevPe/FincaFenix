using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOuputPort
    {
        public bool IsSaved { get; }
        Task Handle(WorkOrderDTO workOrder);
    }
}
