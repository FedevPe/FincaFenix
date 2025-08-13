using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IWorkOrderRepository
    {
        Task<int> AddWorkOrder(WorkOrderEntity workOrder);
        Task<InfoWorkOrderDTO> GetWorkOrderById(int id);
        Task<WorkOrderEntity> GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task<IEnumerable<WorkOrderEntity>> GetAllWorkOrderList();
        Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderList(int pageNumber, int pageSize, string status);
    }
}
