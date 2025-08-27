using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IDetailWorkOrderController
    {
        Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto);
        Task<IEnumerable<DetailWorkOrderDTO>>GetActivitiesByOrderId(int orderId);
    }
}
