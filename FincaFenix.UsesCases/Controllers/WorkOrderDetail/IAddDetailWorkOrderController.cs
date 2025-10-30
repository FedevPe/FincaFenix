using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;

namespace FincaFenix.UsesCases.Controllers.WorkOrderDetail
{
    public interface IAddDetailWorkOrderController
    {
        Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto);
    }
}
