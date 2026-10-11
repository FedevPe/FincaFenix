using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;

namespace FincaFenix.UsesCases.Controllers.Inventory
{
    public interface IInventoryController
    {
        Task<IEnumerable<StockDTO>> GetStockByFarm(int farmId);
        Task<IEnumerable<StockDTO>> GetConsolidatedStock();
        Task<IEnumerable<StockDTO>> GetLowStock();
        Task<IEnumerable<StockDTO>> GetZeroStock();
        Task<IEnumerable<CurrencyDTO>> GetCurrencies();
        Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCosts();
        Task<CostHistoryDTO> GetMaterialCostHistory(int materialId);
        Task<WorkOrderCostDTO> GetWorkOrderCosts(int workOrderId);
        Task<IEnumerable<InventoryMovementDTO>> GetRecentMovements(int take);
        Task<MovementResultDTO> RegisterMovement(RegisterMovementDTO dto);
        Task<ConsumptionResultDTO> RegisterConsumption(RegisterConsumptionDTO dto);
    }
}
