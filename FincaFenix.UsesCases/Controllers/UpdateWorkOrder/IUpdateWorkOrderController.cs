using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.UpdateWorkOrder
{
    public interface IUpdateWorkOrderController
    {
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
