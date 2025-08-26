using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers.GetWorkOrderInformation;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.WorkOrder
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetWorkOrderInformationController(
        IGetWorkOrderInformationInputPort interactor,
        IGetWorkOrderInformationOutputPort presenter) : IGetWorkOrderInformationController
    {
        [HttpGet("getallworkorderinfo")]
        public async Task<List<ShowWorkOrderDTO>> GetAllWorkOrderInfoList()
        {
            await interactor.GetAllWorkOrderList();
            return presenter.InfoWorkOrderList;
        }

        [HttpGet("getCompleteInfoWorkOrder/{id}")]
        public async Task<ShowWorkOrderDTO> GetWorkOrderAndRecipeByIdWorkorder(int id)
        {
            await interactor.GetWorkOrderAndRecipeByIdWorkorder(id);
            return presenter.WorkOrderInfo;
        }

        [HttpGet("order/{id}/getinfo")]
        public async Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id)
        {
            await interactor.GetWorkOrderById(id);
            return presenter.WorkOrder;
        }
        [HttpGet("{pagenumber}/{pagesize}/getworkorderlistpaginated")]
        public async Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
        {
            await interactor.GetWorkOrderListPaginated(pageNumber, pageSize, status);
            return (presenter.InfoWorkOrderList, presenter.TotalCount);
        }
    }
}
