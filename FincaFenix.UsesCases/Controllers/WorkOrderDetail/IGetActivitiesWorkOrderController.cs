using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;

namespace FincaFenix.UsesCases.Controllers.WorkOrderDetail
{
    public interface IGetActivitiesWorkOrderController
    {
        Task<IEnumerable<DetailWorkOrderDTO>> GetActivitiesByOrderId(int orderId);
    }
}
