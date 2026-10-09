using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Entities.Units;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class WorkOrderInventoryCommand(FincaFenixContext context) : IWorkOrderInventoryCommand
    {
        public async Task RegisterReservationsAndCostsAsync(WorkOrderEntity workOrder, ICollection<DetailRecipeEntity> details)
        {
            var detailList = details.ToList();

            var materialIds = detailList.Select(d => d.MaterialId).Distinct().ToList();
            var materials = await context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var stockRows = await context.StockByFarms
                .FromSqlRaw("SELECT * FROM StockPorFinca WITH (UPDLOCK, HOLDLOCK) WHERE IdFinca = {0}", workOrder.FarmId)
                .ToListAsync();

            foreach (var detail in detailList.Where(d => d.EstimatedAmount > 0))
            {
                var plannedAmount = ToBaseUnit(detail, materials);
                var stock = stockRows.FirstOrDefault(s => s.MaterialId == detail.MaterialId);
                var available = stock is null ? 0m : stock.StockFisico - stock.StockReservado;

                if (plannedAmount > available)
                {
                    throw new BusinessRuleException(
                        $"Stock insuficiente del material {detail.MaterialId} en la finca {workOrder.FarmId}. Disponible {available}, requerido {plannedAmount}.");
                }
            }

            var now = DateTime.Now;

            foreach (var detail in detailList)
            {
                materials.TryGetValue(detail.MaterialId, out var material);
                var plannedAmount = ToBaseUnit(detail, materials);

                if (detail.EstimatedAmount > 0)
                {
                    var stock = stockRows.First(s => s.MaterialId == detail.MaterialId);
                    stock.StockReservado += plannedAmount;

                    context.MaterialReservations.Add(new MaterialReservationEntity
                    {
                        WorkOrderId = workOrder.Id,
                        MaterialId = detail.MaterialId,
                        FarmId = workOrder.FarmId,
                        ReservedAmount = plannedAmount,
                        ConsumedAmount = 0,
                        State = nameof(ReservationStateEnum.Activa),
                        CreatedDate = now
                    });
                }

                var unitCost = material?.ReferenceCost ?? 0m;

                context.WorkOrderCosts.Add(new WorkOrderCostEntity
                {
                    WorkOrderId = workOrder.Id,
                    MaterialId = detail.MaterialId,
                    CurrencyId = material?.CurrencyId ?? 1,
                    PlannedAmount = plannedAmount,
                    UnitCost = unitCost,
                    TotalCost = unitCost * plannedAmount,
                    FrozenDate = now
                });
            }

            await context.SaveChangesAsync();
        }

        private static decimal ToBaseUnit(DetailRecipeEntity detail, IReadOnlyDictionary<int, MaterialEntity> materials)
        {
            materials.TryGetValue(detail.MaterialId, out var material);

            return UnitConverter.ConvertToBaseUnit(
                detail.EstimatedAmount,
                detail.EstimatedAmountUnit,
                material?.UnitOfMeasureId);
        }

        public async Task ReleaseReservationsAsync(int workOrderId)
        {
            var workOrder = await context.WorkOrders
                .FirstOrDefaultAsync(w => w.Id == workOrderId);

            if (workOrder is null)
            {
                return;
            }

            var reservations = await context.MaterialReservations
                .Where(r => r.WorkOrderId == workOrderId && r.State == nameof(ReservationStateEnum.Activa))
                .ToListAsync();

            if (reservations.Count == 0)
            {
                return;
            }

            var stockRows = await context.StockByFarms
                .FromSqlRaw("SELECT * FROM StockPorFinca WITH (UPDLOCK, HOLDLOCK) WHERE IdFinca = {0}", workOrder.FarmId)
                .ToListAsync();

            var now = DateTime.Now;

            foreach (var reservation in reservations)
            {
                var pending = reservation.ReservedAmount - reservation.ConsumedAmount;

                if (pending > 0)
                {
                    var stock = stockRows.FirstOrDefault(s => s.MaterialId == reservation.MaterialId);

                    if (stock is not null)
                    {
                        stock.StockReservado -= pending;
                    }
                }

                reservation.State = nameof(ReservationStateEnum.Liberada);
                reservation.ReleasedDate = now;
            }

            await context.SaveChangesAsync();
        }
    }
}
