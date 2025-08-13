using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IWorkOrderInputPort
    {
        Task Handle(WorkOrderDTO workOrder);
        Task GetWorkOrderById(int id);
        Task GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task GetWorkOrderListPaginated(int pageNumber, int pageSize, string status);
        Task GetAllWorkOrderList();
    }
}
