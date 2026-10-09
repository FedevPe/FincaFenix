using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.WorkOrder
{
    public interface IUpdateWorkOrderController
    {
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
        Task<bool> UpdateWorkOrder(UpdateWorkOrderDTO dto);
    }
}
