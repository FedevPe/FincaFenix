using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IWorkOrderController
    {
        Task<bool> CreateWorkOrder(WorkOrderDTO workOrder);
        Task<ShowInfoAddActivityFormDTO> GetWorkOrderInfoById(int id);
        Task<(IEnumerable<InfoWorkOrderDTO>WorkOrders, int TotalAcount)> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status);
    }
}