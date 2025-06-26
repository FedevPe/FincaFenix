using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class CreateWorkOrderViewModel(IWorkOrderController wo) : WorkOrderDTO
    {
        public async Task SaveWorkOrder(WorkOrderDTO newWorkOrder)
        {
            await wo.CreateWorkOrder(newWorkOrder);
        }
    }
}
