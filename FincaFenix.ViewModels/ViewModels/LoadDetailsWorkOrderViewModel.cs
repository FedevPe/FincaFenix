using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class LoadDetailsWorkOrderViewModel(
        IDetailWorkOrderController detail)
    {
        public IEnumerable<ActivityWorkOrderDTO>? ActivityWorkOrderDTOs { get; set; }

        public async Task LoadActivityLog(int orderId)
        {
            ActivityWorkOrderDTOs = await detail.GetActivitiesByOrderId(orderId);
        }
    }
}
