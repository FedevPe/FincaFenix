using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;

namespace FincaFenix.UsesCases.Controllers.WorkOrderDetail
{
    public interface IGetActivitiesWorkOrderController
    {
        Task<IEnumerable<ActivityWorkOrderDTO>> GetActivitiesByOrderId(int orderId);
    }
}
