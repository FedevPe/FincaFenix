using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs;

namespace FincaFenix.Gateways.Interfaces.QueryServices.Inventory
{
    public interface IInventoryQueryService
    {
        Task<IEnumerable<StockDTO>> GetStockByFarmAsync(int farmId);
        Task<IEnumerable<StockDTO>> GetConsolidatedStockAsync();
        Task<IEnumerable<StockDTO>> GetLowStockAsync();
        Task<IEnumerable<StockDTO>> GetZeroStockAsync();
        Task<IEnumerable<CurrencyDTO>> GetCurrenciesAsync();
        Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCostsAsync();
        Task<PagedResult<CurrentMaterialCostDTO>> GetCurrentMaterialCostsPagedAsync(MaterialFilterDTO filter);
        Task<CostHistoryDTO> GetMaterialCostHistoryAsync(int materialId);
        Task<WorkOrderCostDTO> GetWorkOrderCostsAsync(int workOrderId);
        Task<MaterialReservationsDTO> GetMaterialReservationsAsync(int materialId);
        Task<IEnumerable<InventoryMovementDTO>> GetRecentMovementsAsync(int take);
    }
}
