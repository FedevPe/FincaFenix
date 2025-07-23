using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController(
        IWorkOrderInputPort interactor,
        IWorkOrderOutputPort presenter) : IWorkOrderController
    {
        [HttpPost("createworkorder")]
        public async Task<bool> CreateWorkOrder(WorkOrderDTO workOrder)
        {
            await interactor.Handle(workOrder);
            return presenter.IsSaved;
        }
        [HttpGet("order/{id}/getinfo")]
        public async Task<ShowInfoAddActivityFormDTO> GetWorkOrderInfoById(int id)
        {
            await interactor.GetWorkOrderById(id);
            return presenter.WorkOrder;
        }
        [HttpGet("{pagenumber}/{pagesize}/getworkorderlistpaginated")]
        public async Task<(IEnumerable<InfoWorkOrderDTO> WorkOrders, int TotalAcount)> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
        {
            await interactor.GetWorkOrderListPaginated(pageNumber, pageSize, status);
            return (presenter.InfoWorkOrderCard, presenter.TotalCount);
        }
    }
}
