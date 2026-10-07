using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.PDF;
using FincaFenix.UsesCases.Controllers.WorkOrder;
using FincaFenix.UsesCases.UseCases.WorkOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.WorkOrder
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController(ISender mediator, IPdfGenerationService pdfService) : ControllerBase,ICreateWorkOrderController, IGetWorkOrderInformationController, IUpdateWorkOrderController
    {
        [HttpPost("createworkorder")]
        [Authorize(Policy = PolicyMaster.WORKORDER_CREATE)]
        public async Task<OperationResultDTO> CreateWorkOrder(WorkOrderDTO workOrder)
        {
            return await mediator.Send(new CreateWorkOrderCommand(workOrder));
        }

        [HttpGet("getallworkorderinfo")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        public async Task<IEnumerable<ShowWorkOrderDTO>> GetAllWorkOrderInfoList()
        {
            return await mediator.Send(new GetAllWorkOrdersQuery());
        }

        [HttpGet("getCompleteInfoWorkOrder/{id}")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        public async Task<ShowWorkOrderDTO> GetWorkOrderAndRecipeByIdWorkorder(int id)
        {
            return await mediator.Send(new GetWorkOrderByIdQuery(id));
        }

        [HttpGet("order/{id}/getinfo")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        public async Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id)
        {
            return await mediator.Send(new GetWorkOrderInfoByIdQuery(id));
        }

        [HttpGet("{pagenumber}/{pagesize}/getworkorderlistpaginated")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        public async Task<PagedResult<ShowWorkOrderDTO>> GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
        {
            return await mediator.Send(new GetWorkOrderListPaginatedQuery(pageNumber, pageSize, status));
        }

        [HttpPost("updatestateworkorder")]
        [Authorize(Policy = PolicyMaster.WORKORDER_UPDATE)]
        public async Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            return await mediator.Send(new UpdateWorkOrderStateCommand(workOrderId, newStatus));
        }

        [HttpGet("{id}/pdf")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        public async Task<IActionResult> GetWorkOrderPdf(int id)
        {
            var dto = await mediator.Send(new GetWorkOrderByIdQuery(id));
            var pdfBytes = pdfService.GenerateWorkOrderPdf(dto);
            return File(pdfBytes, "application/pdf", $"Orden_de_trabajo_N°_{dto.OrderNum}.pdf");
        }
    }
}
