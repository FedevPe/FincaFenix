using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Gateways.Interfaces.QueryServices.Inventory;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices.Inventory
{
    public class InventoryQueryService(FincaFenixContext context) : IInventoryQueryService
    {
        public async Task<IEnumerable<StockDTO>> GetStockByFarmAsync(int farmId)
        {
            return await context.StockByFarms
                .AsNoTracking()
                .Where(s => s.FarmId == farmId)
                .Select(s => new StockDTO
                {
                    MaterialId = s.MaterialId,
                    ArticleName = s.Material.ArticleName,
                    CommercialName = s.Material.CommercialName,
                    UnitOfMeasure = s.Material.UnitOfMeasure.Description,
                    FarmId = s.FarmId,
                    FarmName = s.Farm.Name,
                    StockFisico = s.StockFisico,
                    StockReservado = s.StockReservado,
                    StockDisponible = s.StockFisico - s.StockReservado,
                    StockMinimo = s.StockMinimo,
                    IsLow = s.StockMinimo > 0 && (s.StockFisico - s.StockReservado) <= s.StockMinimo
                })
                .OrderBy(s => s.ArticleName)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockDTO>> GetConsolidatedStockAsync()
        {
            return await context.StockByFarms
                .AsNoTracking()
                .GroupBy(s => new
                {
                    s.MaterialId,
                    s.Material.ArticleName,
                    s.Material.CommercialName,
                    UnitDescription = s.Material.UnitOfMeasure.Description
                })
                .Select(g => new StockDTO
                {
                    MaterialId = g.Key.MaterialId,
                    ArticleName = g.Key.ArticleName,
                    CommercialName = g.Key.CommercialName,
                    UnitOfMeasure = g.Key.UnitDescription,
                    FarmId = 0,
                    FarmName = null,
                    StockFisico = g.Sum(x => x.StockFisico),
                    StockReservado = g.Sum(x => x.StockReservado),
                    StockDisponible = g.Sum(x => x.StockFisico) - g.Sum(x => x.StockReservado),
                    StockMinimo = g.Sum(x => x.StockMinimo),
                    IsLow = g.Sum(x => x.StockMinimo) > 0
                        && (g.Sum(x => x.StockFisico) - g.Sum(x => x.StockReservado)) <= g.Sum(x => x.StockMinimo)
                })
                .OrderBy(s => s.ArticleName)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockDTO>> GetLowStockAsync()
        {
            return await context.StockByFarms
                .AsNoTracking()
                .Where(s => s.StockMinimo > 0 && (s.StockFisico - s.StockReservado) <= s.StockMinimo)
                .Select(s => new StockDTO
                {
                    MaterialId = s.MaterialId,
                    ArticleName = s.Material.ArticleName,
                    CommercialName = s.Material.CommercialName,
                    UnitOfMeasure = s.Material.UnitOfMeasure.Description,
                    FarmId = s.FarmId,
                    FarmName = s.Farm.Name,
                    StockFisico = s.StockFisico,
                    StockReservado = s.StockReservado,
                    StockDisponible = s.StockFisico - s.StockReservado,
                    StockMinimo = s.StockMinimo,
                    IsLow = true
                })
                .OrderBy(s => s.ArticleName)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockDTO>> GetZeroStockAsync()
        {
            return await context.StockByFarms
                .AsNoTracking()
                .Where(s => s.StockFisico <= 0)
                .Select(s => new StockDTO
                {
                    MaterialId = s.MaterialId,
                    ArticleName = s.Material.ArticleName,
                    CommercialName = s.Material.CommercialName,
                    UnitOfMeasure = s.Material.UnitOfMeasure.Description,
                    FarmId = s.FarmId,
                    FarmName = s.Farm.Name,
                    StockFisico = s.StockFisico,
                    StockReservado = s.StockReservado,
                    StockDisponible = s.StockFisico - s.StockReservado,
                    StockMinimo = s.StockMinimo,
                    IsLow = s.StockMinimo > 0
                })
                .OrderBy(s => s.ArticleName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CurrencyDTO>> GetCurrenciesAsync()
        {
            return await context.Currencies
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Code)
                .Select(c => new CurrencyDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Symbol = c.Symbol
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CurrentMaterialCostDTO>> GetCurrentMaterialCostsAsync()
        {
            return await context.Materials
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Select(m => new CurrentMaterialCostDTO
                {
                    MaterialId = m.Id,
                    ArticleName = m.ArticleName,
                    CommercialName = m.CommercialName,
                    UnitOfMeasure = m.UnitOfMeasure != null ? m.UnitOfMeasure.Description : null,
                    CategoryId = m.CategoryId,
                    CategoryName = m.Category != null ? m.Category.Description : null,
                    ReferenceCost = m.ReferenceCost,
                    CurrencyId = m.CurrencyId,
                    CurrencyCode = m.Currency.Code,
                    CurrencySymbol = m.Currency.Symbol
                })
                .OrderBy(m => m.ArticleName)
                .ToListAsync();
        }

        public async Task<PagedResult<CurrentMaterialCostDTO>> GetCurrentMaterialCostsPagedAsync(MaterialFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Materials
                .AsNoTracking()
                .Where(m => !m.IsDeleted);

            if (filter.CategoryId.HasValue)
                baseQuery = baseQuery.Where(m => m.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(m =>
                    m.ArticleName.Contains(search) ||
                    m.CommercialName.Contains(search) ||
                    m.CodeSap.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .OrderBy(m => m.ArticleName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new CurrentMaterialCostDTO
                {
                    MaterialId = m.Id,
                    ArticleName = m.ArticleName,
                    CommercialName = m.CommercialName,
                    UnitOfMeasure = m.UnitOfMeasure != null ? m.UnitOfMeasure.Description : null,
                    CategoryId = m.CategoryId,
                    CategoryName = m.Category != null ? m.Category.Description : null,
                    ReferenceCost = m.ReferenceCost,
                    CurrencyId = m.CurrencyId,
                    CurrencyCode = m.Currency.Code,
                    CurrencySymbol = m.Currency.Symbol
                })
                .ToListAsync();

            return new PagedResult<CurrentMaterialCostDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CostHistoryDTO> GetMaterialCostHistoryAsync(int materialId)
        {
            var material = await context.Materials
                .AsNoTracking()
                .Include(m => m.UnitOfMeasure)
                .Include(m => m.Currency)
                .FirstOrDefaultAsync(m => m.Id == materialId && !m.IsDeleted);

            if (material is null)
                throw new NotFoundException($"No se encontró el material {materialId}.");

            var items = await context.InventoryMovements
                .AsNoTracking()
                .Where(mv => mv.MaterialId == materialId && mv.UnitCost.HasValue)
                .OrderByDescending(mv => mv.Date)
                .Select(mv => new CostHistoryItemDTO
                {
                    MovementId = mv.Id,
                    MovementType = mv.MovementType,
                    Origin = mv.Origin,
                    Amount = mv.Amount,
                    UnitCost = mv.UnitCost,
                    TotalCost = mv.TotalCost,
                    FarmId = mv.FarmId,
                    FarmName = mv.Farm.Name,
                    Date = mv.Date
                })
                .ToListAsync();

            return new CostHistoryDTO
            {
                MaterialId = material.Id,
                ArticleName = material.ArticleName,
                CommercialName = material.CommercialName,
                UnitOfMeasure = material.UnitOfMeasure != null ? material.UnitOfMeasure.Description : null,
                ReferenceCost = material.ReferenceCost,
                CurrencyCode = material.Currency != null ? material.Currency.Code : null,
                CurrencySymbol = material.Currency != null ? material.Currency.Symbol : null,
                Items = items
            };
        }

        public async Task<WorkOrderCostDTO> GetWorkOrderCostsAsync(int workOrderId)
        {
            var workOrder = await context.WorkOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == workOrderId && !w.IsDeleted);

            if (workOrder is null)
                throw new NotFoundException($"No se encontró la orden de trabajo {workOrderId}.");

            var consumedByMaterial = await context.Consumptions
                .AsNoTracking()
                .Where(c => c.WorkOrderId == workOrderId)
                .ToDictionaryAsync(c => c.MaterialId, c => c.ConsumedAmount);

            var items = await context.WorkOrderCosts
                .AsNoTracking()
                .Where(c => c.WorkOrderId == workOrderId)
                .Select(c => new WorkOrderCostItemDTO
                {
                    Id = c.Id,
                    MaterialId = c.MaterialId,
                    MaterialName = c.Material.ArticleName,
                    UnitOfMeasure = c.Material.UnitOfMeasure != null ? c.Material.UnitOfMeasure.Description : null,
                    PlannedAmount = c.PlannedAmount,
                    UnitCost = c.UnitCost,
                    TotalCost = c.TotalCost,
                    CurrencyCode = c.Currency.Code,
                    CurrencySymbol = c.Currency.Symbol,
                    FrozenDate = c.FrozenDate
                })
                .OrderBy(c => c.MaterialId)
                .ToListAsync();

            foreach (var item in items)
            {
                item.ConsumedAmount = consumedByMaterial.TryGetValue(item.MaterialId, out var consumed)
                    ? consumed
                    : 0m;
                item.RealCost = item.ConsumedAmount * item.UnitCost;
            }

            return new WorkOrderCostDTO
            {
                WorkOrderId = workOrder.Id,
                OrderNum = workOrder.OrderNum,
                TotalCost = items.Sum(i => i.TotalCost),
                TotalRealCost = items.Sum(i => i.RealCost),
                Items = items
            };
        }

        public async Task<MaterialReservationsDTO> GetMaterialReservationsAsync(int materialId)
        {
            var material = await context.Materials
                .AsNoTracking()
                .Include(m => m.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.Id == materialId && !m.IsDeleted);

            if (material is null)
                throw new NotFoundException($"No se encontró el material {materialId}.");

            var items = await context.MaterialReservations
                .AsNoTracking()
                .Where(r => r.MaterialId == materialId)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new ReservationItemDTO
                {
                    ReservationId = r.Id,
                    WorkOrderId = r.WorkOrderId,
                    OrderNum = r.WorkOrder.OrderNum,
                    WorkOrderStatus = r.WorkOrder.Status,
                    FarmId = r.FarmId,
                    FarmName = r.Farm.Name,
                    ReservedAmount = r.ReservedAmount,
                    ConsumedAmount = r.ConsumedAmount,
                    PendingAmount = r.ReservedAmount - r.ConsumedAmount,
                    State = r.State,
                    CreatedDate = r.CreatedDate,
                    ReleasedDate = r.ReleasedDate
                })
                .ToListAsync();

            return new MaterialReservationsDTO
            {
                MaterialId = material.Id,
                ArticleName = material.ArticleName,
                CommercialName = material.CommercialName,
                UnitOfMeasure = material.UnitOfMeasure != null ? material.UnitOfMeasure.Description : null,
                Items = items
            };
        }

        public async Task<IEnumerable<InventoryMovementDTO>> GetRecentMovementsAsync(int take)
        {
            var limit = take < 1 ? 10 : take;

            return await context.InventoryMovements
                .AsNoTracking()
                .OrderByDescending(mv => mv.Date)
                .ThenByDescending(mv => mv.Id)
                .Take(limit)
                .Select(mv => new InventoryMovementDTO
                {
                    Id = mv.Id,
                    MaterialId = mv.MaterialId,
                    ArticleName = mv.Material.ArticleName,
                    CommercialName = mv.Material.CommercialName,
                    UnitOfMeasure = mv.Material.UnitOfMeasure != null ? mv.Material.UnitOfMeasure.Description : null,
                    FarmId = mv.FarmId,
                    FarmName = mv.Farm.Name,
                    MovementType = mv.MovementType,
                    Origin = mv.Origin,
                    Amount = mv.Amount,
                    UnitCost = mv.UnitCost,
                    TotalCost = mv.TotalCost,
                    CurrencyCode = mv.Currency != null ? mv.Currency.Code : null,
                    CurrencySymbol = mv.Currency != null ? mv.Currency.Symbol : null,
                    ResultingStock = mv.ResultingStock,
                    Date = mv.Date,
                    Observations = mv.Observations,
                    WorkOrderId = mv.WorkOrderId,
                    OrderNum = mv.WorkOrder != null ? mv.WorkOrder.OrderNum : null
                })
                .ToListAsync();
        }
    }
}
