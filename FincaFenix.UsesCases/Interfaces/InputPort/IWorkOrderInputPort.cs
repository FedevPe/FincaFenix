using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Aggregates;

namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IWorkOrderInputPort
    {
        Task Handle(WorkOrderDTO workOrder);
    }
}
