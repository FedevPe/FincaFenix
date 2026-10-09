using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Entities.Units;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class WorkOrderConsumptionCommand(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IWorkOrderConsumptionCommand
    {
        public async Task ApplyAutomaticConsumptionAsync(int workOrderId)
        {
            var workOrder = await context.WorkOrders
                .Include(w => w.Recipe)
                    .ThenInclude(r => r.DetailRecipeList)
                .FirstOrDefaultAsync(w => w.Id == workOrderId && !w.IsDeleted);

            if (workOrder?.Recipe?.DetailRecipeList is not { Count: > 0 })
            {
                return;
            }

            var totalPerformance = await context.DetailWorkOrders
                .Where(d => d.WorkOrderId == workOrderId)
                .SumAsync(d => (decimal?)d.Performance) ?? 0m;

            var stockRows = await LockStockRowsAsync(workOrder.FarmId);

            foreach (var detail in workOrder.Recipe.DetailRecipeList)
            {
                var dose = detail.AmountRequired;
                var target = totalPerformance * dose;

                if (target <= 0)
                {
                    continue;
                }

                await ApplyConsumptionAsync(
                    workOrder,
                    stockRows,
                    detail.MaterialId,
                    target,
                    isCumulative: true,
                    detail.AmountRequiredUnit,
                    ConsumptionOriginEnum.Calculado,
                    userId: null,
                    observations: null);
            }

            await context.SaveChangesAsync();
        }

        public async Task<ConsumptionResultDTO> RegisterManualConsumptionAsync(RegisterConsumptionDTO dto)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var workOrder = await context.WorkOrders
                    .FirstOrDefaultAsync(w => w.Id == dto.WorkOrderId && !w.IsDeleted);

                if (workOrder is null)
                {
                    throw new NotFoundException($"No existe la orden de trabajo {dto.WorkOrderId}.");
                }

                var stockRows = await LockStockRowsAsync(workOrder.FarmId);

                var result = await ApplyConsumptionAsync(
                    workOrder,
                    stockRows,
                    dto.MaterialId,
                    dto.Amount,
                    isCumulative: false,
                    dto.Unit,
                    ConsumptionOriginEnum.Manual,
                    dto.UserId,
                    dto.Observations);

                await context.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task<ConsumptionResultDTO> ApplyConsumptionAsync(
            WorkOrderEntity workOrder,
            IReadOnlyList<StockByFarmEntity> stockRows,
            int materialId,
            decimal amount,
            bool isCumulative,
            string? unit,
            ConsumptionOriginEnum origin,
            int? userId,
            string? observations)
        {
            var material = await context.Materials.FirstOrDefaultAsync(m => m.Id == materialId);
            if (material is null)
            {
                throw new NotFoundException($"No existe el material {materialId}.");
            }

            var baseAmount = UnitConverter.ConvertToBaseUnit(amount, unit, material.UnitOfMeasureId);

            string? unitOfMeasure = null;

            if (material.UnitOfMeasureId.HasValue
                && UnitOfMeasureCatalog.CodeById.TryGetValue(material.UnitOfMeasureId.Value, out var baseUnitCode))
            {
                unitOfMeasure = baseUnitCode;
            }

            var consumption = await context.Consumptions
                .FirstOrDefaultAsync(c => c.WorkOrderId == workOrder.Id && c.MaterialId == materialId);

            var consumedBefore = consumption?.ConsumedAmount ?? 0m;
            var appliedBefore = consumption?.AppliedAmount ?? 0m;

            var delta = isCumulative ? baseAmount - consumedBefore : baseAmount;
            var resulting = isCumulative ? baseAmount : consumedBefore + baseAmount;

            if (delta <= 0)
            {
                return new ConsumptionResultDTO
                {
                    ConsumptionId = consumption?.Id ?? 0,
                    ConsumedAmount = consumedBefore,
                    AppliedAmount = 0m,
                    ExcessAmount = consumedBefore - appliedBefore,
                    UnitOfMeasure = unitOfMeasure ?? unit
                };
            }

            var reservation = await context.MaterialReservations
                .FirstOrDefaultAsync(r => r.WorkOrderId == workOrder.Id
                    && r.MaterialId == materialId
                    && r.State == nameof(ReservationStateEnum.Activa));

            var stock = stockRows.FirstOrDefault(s => s.MaterialId == materialId);
            var reservationRemaining = reservation is null
                ? 0m
                : Math.Max(reservation.ReservedAmount - reservation.ConsumedAmount, 0m);
            var stockAvailable = stock is null
                ? 0m
                : Math.Max(stock.StockFisico - stock.StockReservado, 0m);

            var fromReservation = Math.Min(delta, reservationRemaining);
            var fromStock = Math.Min(delta - fromReservation, stockAvailable);
            var applied = fromReservation + fromStock;

            if (reservation is not null && fromReservation > 0)
            {
                reservation.ConsumedAmount += fromReservation;

                if (reservation.ConsumedAmount >= reservation.ReservedAmount)
                {
                    reservation.State = nameof(ReservationStateEnum.Consumida);
                }
            }

            if (stock is not null && applied > 0)
            {
                var previousStock = stock.StockFisico;
                stock.StockFisico -= applied;
                stock.StockReservado -= fromReservation;

                var unitCost = material.ReferenceCost;

                context.InventoryMovements.Add(new InventoryMovementEntity
                {
                    MaterialId = materialId,
                    FarmId = workOrder.FarmId,
                    MovementType = (origin == ConsumptionOriginEnum.Manual
                        ? InventoryMovementTypeEnum.SalidaManual
                        : InventoryMovementTypeEnum.SalidaConsumo).ToString(),
                    Amount = applied,
                    UnitCost = unitCost,
                    TotalCost = unitCost.HasValue ? unitCost.Value * applied : null,
                    CurrencyId = material.CurrencyId,
                    PreviousStock = previousStock,
                    ResultingStock = stock.StockFisico,
                    Date = DateTime.Now,
                    UserId = userId,
                    Origin = (origin == ConsumptionOriginEnum.Manual
                        ? InventoryOriginEnum.Manual
                        : InventoryOriginEnum.OrdenTrabajo).ToString(),
                    WorkOrderId = workOrder.Id,
                    Observations = observations
                });
            }

            if (consumption is null)
            {
                consumption = new ConsumptionEntity
                {
                    WorkOrderId = workOrder.Id,
                    MaterialId = materialId,
                    AppliedAmount = 0m
                };
                context.Consumptions.Add(consumption);
            }

            consumption.ConsumedAmount = resulting;
            consumption.AppliedAmount += applied;
            consumption.Unit = unitOfMeasure ?? unit;
            consumption.Origin = origin.ToString();
            consumption.UnitCost = material.ReferenceCost;
            consumption.CurrencyId = material.CurrencyId;
            consumption.CalculatedDate = DateTime.Now;

            if (userId.HasValue)
            {
                consumption.UserId = userId;
            }

            return new ConsumptionResultDTO
            {
                ConsumptionId = consumption.Id,
                ConsumedAmount = consumption.ConsumedAmount,
                AppliedAmount = applied,
                ExcessAmount = consumption.ConsumedAmount - consumption.AppliedAmount,
                UnitOfMeasure = unitOfMeasure ?? unit
            };
        }

        private async Task<List<StockByFarmEntity>> LockStockRowsAsync(int farmId)
        {
            return await context.StockByFarms
                .FromSqlRaw("SELECT * FROM StockPorFinca WITH (UPDLOCK, HOLDLOCK) WHERE IdFinca = {0}", farmId)
                .ToListAsync();
        }
    }
}
