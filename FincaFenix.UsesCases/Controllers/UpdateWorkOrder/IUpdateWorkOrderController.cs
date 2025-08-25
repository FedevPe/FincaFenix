using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.UpdateWorkOrder
{
    public interface IUpdateWorkOrderController
    {
        Task<bool> UpdateWorkOrderState(WorkOrderDTO workOrder);
    }
}
