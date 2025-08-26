using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.WorkOrder
{
    public interface IGetWorkOrderInformationRepository
    {
        Task<InfoWorkOrderDTO> GetWorkOrderById(int id);
        Task<WorkOrderEntity> GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task<IEnumerable<WorkOrderEntity>> GetAllWorkOrderList();
        Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderList(int pageNumber, int pageSize, string status);
    }
}
