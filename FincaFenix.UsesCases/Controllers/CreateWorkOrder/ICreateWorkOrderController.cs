using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.CreateWorkOrder
{
    public interface ICreateWorkOrderController
    {
        Task<bool> CreateWorkOrder(WorkOrderDTO workOrder);
    }
}
