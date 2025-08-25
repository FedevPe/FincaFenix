using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class GetWorkOrderInformationRepository(
        IGetWorkOrderInformationQuery query) : IGetWorkOrderInformationRepository
    {
        public async Task<IEnumerable<WorkOrderEntity>> GetAllWorkOrderList()
        {
            return await query.GetAllWorkOrderList();
        }

        public async Task<WorkOrderEntity> GetWorkOrderAndRecipeByIdWorkorder(int id)
        {
            return await query.GetWorkOrderAndRecipeByIdWorkorder(id);
        }

        public async Task<InfoWorkOrderDTO> GetWorkOrderById(int id)
        {
            return await query.GetWorkOrderInfoById(id);
        }

        public async Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderList(int pageNumber, int pageSize, string status)
        {
            return await query.GetWorkOrderListPaged(pageNumber, pageSize, status);
        }
    }
}
