using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers.GetWorkOrderInformation
{
    public interface IGetWorkOrderInformationController
    {
        Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id);
        Task<ShowWorkOrderDTO> GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task<List<ShowWorkOrderDTO>> GetAllWorkOrderInfoList();
        Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status);
    }
}
