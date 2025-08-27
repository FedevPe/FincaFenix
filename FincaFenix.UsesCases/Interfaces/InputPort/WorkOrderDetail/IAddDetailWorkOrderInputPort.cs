using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;

namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail
{
    public interface IAddDetailWorkOrderInputPort
    {
        Task AddDetailWorkOrder(AddDetailWorkOrderDTO dto);
        Task GetActivitiesByOrderId(int orderId);
    }
}
