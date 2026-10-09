using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.Inventory
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<StockDTO>> GetStockByFarmAsync(int farmId);
        Task<IEnumerable<StockDTO>> GetConsolidatedStockAsync();
        Task<IEnumerable<StockDTO>> GetLowStockAsync();
        Task<IEnumerable<StockDTO>> GetZeroStockAsync();
        Task<IEnumerable<CurrencyDTO>> GetCurrenciesAsync();
        Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCostsAsync();
        Task<CostHistoryDTO> GetMaterialCostHistoryAsync(int materialId);
        Task<WorkOrderCostDTO> GetWorkOrderCostsAsync(int workOrderId);
        Task<MovementResultDTO> RegisterMovementAsync(InventoryMovementEntity movement, decimal? stockMinimum);
        Task<ConsumptionResultDTO> RegisterConsumptionAsync(RegisterConsumptionDTO dto);
    }
}
