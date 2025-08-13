using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IWorkOrderQueryService
    {
        Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id);
        Task<WorkOrderEntity> GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task<IEnumerable<WorkOrderEntity>> GetAllWorkOrderList();
        Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalCount)> GetWorkOrderListPaged(int pageNumber, int pageSize, string status);
    }
}