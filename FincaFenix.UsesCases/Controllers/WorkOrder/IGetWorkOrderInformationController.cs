using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.WorkOrder
{
    public interface IGetWorkOrderInformationController
    {
        Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id);
        Task<ShowWorkOrderDTO> GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task<IEnumerable<ShowWorkOrderDTO>> GetAllWorkOrderInfoList();
        Task<PagedResult<ShowWorkOrderDTO>> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status);
    }
}
