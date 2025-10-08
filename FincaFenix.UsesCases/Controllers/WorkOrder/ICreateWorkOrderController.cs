using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.WorkOrder
{
    public interface ICreateWorkOrderController
    {
        Task<OperationResultDTO> CreateWorkOrder(WorkOrderDTO workOrder);
    }
}
