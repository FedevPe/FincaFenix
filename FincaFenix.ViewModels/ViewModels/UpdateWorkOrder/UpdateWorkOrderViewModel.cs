using FincaFenix.UsesCases.Controllers.WorkOrder;

namespace FincaFenix.ViewModels.ViewModels.UpdateWorkOrder
{
    public class UpdateWorkOrderViewModel(
        IUpdateWorkOrderController controller)
    {
        public async Task<bool> UpdateWorkOrderStatus(int workOrderId, string newStatus)
        {
            try
            {
                return await controller.UpdateWorkOrderState(workOrderId, newStatus);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
