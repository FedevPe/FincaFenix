using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IDetailWorkOrderController
    {
        Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto);
        Task<IEnumerable<ActivityWorkOrderDTO>>GetActivitiesByOrderId(int orderId);
    }
}
