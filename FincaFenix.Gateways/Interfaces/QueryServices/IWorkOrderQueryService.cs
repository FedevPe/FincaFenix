using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IWorkOrderQueryService
    {
        Task<ShowInfoAddActivityFormDTO> GetWorkOrderInfoById(int id);
        Task<(IEnumerable<ShowWorkOrderCardDTO> WorkOrders, int TotalCount)> GetWorkOrderListPaged(int pageNumber, int pageSize);
    }
}
