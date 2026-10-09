using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Inventory;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.Gateways.Interfaces.QueryServices.Inventory;
using FincaFenix.UsesCases.Repository.Inventory;

namespace FincaFenix.Gateways.Implementations.Inventory
{
    public class InventoryRepository(
        IInventoryQueryService queryService,
        IInventoryCommandService commandService,
        IWorkOrderConsumptionCommand consumptionCommand) : IInventoryRepository
    {
        public Task<IEnumerable<StockDTO>> GetStockByFarmAsync(int farmId)
            => queryService.GetStockByFarmAsync(farmId);

        public Task<IEnumerable<StockDTO>> GetConsolidatedStockAsync()
            => queryService.GetConsolidatedStockAsync();

        public Task<IEnumerable<StockDTO>> GetLowStockAsync()
            => queryService.GetLowStockAsync();

        public Task<IEnumerable<StockDTO>> GetZeroStockAsync()
            => queryService.GetZeroStockAsync();

        public Task<IEnumerable<CurrencyDTO>> GetCurrenciesAsync()
            => queryService.GetCurrenciesAsync();

        public Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCostsAsync()
            => queryService.GetCurrentMaterialCostsAsync();

        public Task<CostHistoryDTO> GetMaterialCostHistoryAsync(int materialId)
            => queryService.GetMaterialCostHistoryAsync(materialId);

        public Task<WorkOrderCostDTO> GetWorkOrderCostsAsync(int workOrderId)
            => queryService.GetWorkOrderCostsAsync(workOrderId);

        public Task<MovementResultDTO> RegisterMovementAsync(InventoryMovementEntity movement, decimal? stockMinimum)
            => commandService.RegisterMovementAsync(movement, stockMinimum);

        public Task<ConsumptionResultDTO> RegisterConsumptionAsync(RegisterConsumptionDTO dto)
            => consumptionCommand.RegisterManualConsumptionAsync(dto);
    }
}
