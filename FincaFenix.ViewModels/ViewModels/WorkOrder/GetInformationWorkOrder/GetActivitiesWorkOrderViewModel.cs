using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;

namespace FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder
{
    public class GetActivitiesWorkOrderViewModel(
        IGetActivitiesWorkOrderController detail)
    {
        public IEnumerable<ActivityWorkOrderDTO>? ActivityWorkOrderDTOs { get; set; }

        public async Task LoadActivityLog(int orderId)
        {
            ActivityWorkOrderDTOs = await detail.GetActivitiesByOrderId(orderId);
        }
    }
}
