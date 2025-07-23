using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class WorkOrderRepository(
        IWorkOrderCommandService commandService,
        IWorkOrderQueryService queryService) : IWorkOrderRepository
    {
        public async Task<int> AddWorkOrder(WorkOrderEntity workOrder)
        {
            return await commandService.SaveWorkOrder(workOrder);
        }

        public async Task<ShowInfoAddActivityFormDTO> GetWorkOrderById(int id)
        {
            return await queryService.GetWorkOrderInfoById(id);
        }

        public async Task<(IEnumerable<InfoWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderList(int pageNumber, int pageSize, string status)
        {
            return await queryService.GetWorkOrderListPaged(pageNumber, pageSize, status);
        }
    }
}
