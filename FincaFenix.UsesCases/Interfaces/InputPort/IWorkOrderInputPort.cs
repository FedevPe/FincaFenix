using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IWorkOrderInputPort
    {
        Task Handle(WorkOrderDTO workOrder);
        Task GetWorkOrderList(int farmId, string state = "Activo");
    }
}
