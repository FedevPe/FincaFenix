using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class AddActivityWorkOrderFormViewModel (
        IWorkOrderController workOrder,
        ITaskController task,
        IFarmController farm)
    {
        public WorkOrderDTO WorkOrderSelected { get; set; }
        public IEnumerable<WorkOrderDTO> WorkOrderList { get; set; }
        public TaskDTO TaskWorkOrder { get; set; }

        public async Task GetTaskByWorkOrderId(int woIdTask)
        {
            TaskWorkOrder = await task.DisplayTaskById(woIdTask);
        }
    }
}
