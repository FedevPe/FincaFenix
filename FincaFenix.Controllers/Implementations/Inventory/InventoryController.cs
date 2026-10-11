using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs;
using FincaFenix.UsesCases.Controllers.Inventory;
using FincaFenix.UsesCases.UseCases.Inventory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.Inventory
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController(ISender mediator) : ControllerBase, IInventoryController
    {
        [HttpGet("stock/farm/{farmId}")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<StockDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<StockDTO>> GetStockByFarm(int farmId)
        {
            return await mediator.Send(new GetStockByFarmQuery(farmId));
        }

        [HttpGet("stock/consolidated")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<StockDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<StockDTO>> GetConsolidatedStock()
        {
            return await mediator.Send(new GetConsolidatedStockQuery());
        }

        [HttpGet("stock/low")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<StockDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<StockDTO>> GetLowStock()
        {
            return await mediator.Send(new GetLowStockQuery());
        }

        [HttpGet("stock/zero")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<StockDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<StockDTO>> GetZeroStock()
        {
            return await mediator.Send(new GetZeroStockQuery());
        }

        [HttpPost("movement")]
        [Authorize(Policy = PolicyMaster.MOVEMENT_CREATE)]
        [ProducesResponseType(typeof(MovementResultDTO), (int)HttpStatusCode.OK)]
        public async Task<MovementResultDTO> RegisterMovement(RegisterMovementDTO dto)
        {
            return await mediator.Send(new RegisterMovementCommand(dto));
        }

        [HttpPost("consumption")]
        [Authorize(Policy = PolicyMaster.CONSUMPTION_CREATE)]
        [ProducesResponseType(typeof(ConsumptionResultDTO), (int)HttpStatusCode.OK)]
        public async Task<ConsumptionResultDTO> RegisterConsumption(RegisterConsumptionDTO dto)
        {
            return await mediator.Send(new RegisterConsumptionCommand(dto));
        }

        [HttpGet("currencies")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<CurrencyDTO>> GetCurrencies()
        {
            return await mediator.Send(new GetCurrenciesQuery());
        }

        [HttpGet("movements/recent")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<InventoryMovementDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<InventoryMovementDTO>> GetRecentMovements([FromQuery] int take = 10)
        {
            return await mediator.Send(new GetRecentMovementsQuery(take));
        }

        [HttpGet("costs/current")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(IEnumerable<CurrentMaterialCostDTO>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCosts()
        {
            return await mediator.Send(new GetCurrentMaterialCostsQuery());
        }

        [HttpGet("costs/current/paged")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(PagedResult<CurrentMaterialCostDTO>), (int)HttpStatusCode.OK)]
        public async Task<PagedResult<CurrentMaterialCostDTO>> GetCurrentMaterialCostsPaged([FromQuery] MaterialFilterDTO filter)
        {
            return await mediator.Send(new GetCurrentMaterialCostsPagedQuery(filter));
        }

        [HttpGet("reservations/material/{materialId}")]
        [Authorize(Policy = PolicyMaster.STOCK_READ)]
        [ProducesResponseType(typeof(MaterialReservationsDTO), (int)HttpStatusCode.OK)]
        public async Task<MaterialReservationsDTO> GetMaterialReservations(int materialId)
        {
            return await mediator.Send(new GetMaterialReservationsQuery(materialId));
        }

        [HttpGet("costs/material/{materialId}/history")]
        [Authorize(Policy = PolicyMaster.MOVEMENT_READ)]
        [ProducesResponseType(typeof(CostHistoryDTO), (int)HttpStatusCode.OK)]
        public async Task<CostHistoryDTO> GetMaterialCostHistory(int materialId)
        {
            return await mediator.Send(new GetMaterialCostHistoryQuery(materialId));
        }

        [HttpGet("costs/workorder/{workOrderId}")]
        [Authorize(Policy = PolicyMaster.WORKORDER_READ)]
        [ProducesResponseType(typeof(WorkOrderCostDTO), (int)HttpStatusCode.OK)]
        public async Task<WorkOrderCostDTO> GetWorkOrderCosts(int workOrderId)
        {
            return await mediator.Send(new GetWorkOrderCostsQuery(workOrderId));
        }
    }
}
