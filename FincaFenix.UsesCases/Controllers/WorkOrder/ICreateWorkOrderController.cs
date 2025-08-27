using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.WorkOrder
{
    public interface ICreateWorkOrderController
    {
        Task<bool> CreateWorkOrder(WorkOrderDTO workOrder);
    }
}
