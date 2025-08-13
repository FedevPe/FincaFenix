using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class LoadWorkOrderViewModel(
        IWorkOrderController wo)
    {
        public ShowWorkOrderDTO WorkOrder { get; set; } = new ShowWorkOrderDTO();

        public async Task LoadWorkOrderByIdAsync(int id)
        {
            WorkOrder = await wo.GetWorkOrderAndRecipeByIdWorkorder(id);
        }
    }
}
