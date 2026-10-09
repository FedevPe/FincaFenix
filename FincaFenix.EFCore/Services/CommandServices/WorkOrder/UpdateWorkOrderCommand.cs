using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Entities.Units;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class UpdateWorkOrderCommand(
        FincaFenixContext context,
        IUnitOfWork unitOfWork,
        IWorkOrderInventoryCommand workOrderInventoryCommand) : IUpdateWorkOrderCommand
    {
        private static readonly Dictionary<string, string[]> AllowedTransitions = new()
        {
            [nameof(WorkOrderStatusEnum.Pendiente)] = new[] { nameof(WorkOrderStatusEnum.Activo), nameof(WorkOrderStatusEnum.Cerrado), nameof(WorkOrderStatusEnum.Cancelado) },
            [nameof(WorkOrderStatusEnum.Activo)] = new[] { nameof(WorkOrderStatusEnum.Cerrado), nameof(WorkOrderStatusEnum.Cancelado) },
            [nameof(WorkOrderStatusEnum.Cerrado)] = Array.Empty<string>(),
            [nameof(WorkOrderStatusEnum.Cancelado)] = Array.Empty<string>()
        };

        public async Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                throw new BusinessRuleException("El nuevo estado es obligatorio.");
            }

            var workOrder = await context.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == workOrderId);

            if (workOrder == null)
            {
                throw new NotFoundException($"No existe la orden de trabajo {workOrderId}.");
            }

            var currentStatus = workOrder.Status ?? nameof(WorkOrderStatusEnum.Pendiente);

            if (!AllowedTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatus))
            {
                throw new BusinessRuleException($"Transición de estado no permitida: '{currentStatus}' → '{newStatus}'.");
            }

            if (newStatus == nameof(WorkOrderStatusEnum.Cancelado))
            {
                await ValidateCancellableAsync(workOrderId);
            }

            await unitOfWork.BeginAsync();
            try
            {
                workOrder.Status = newStatus;

                if (newStatus == nameof(WorkOrderStatusEnum.Cerrado))
                {
                    workOrder.EndDate = DateTime.Now;
                }

                if (newStatus is nameof(WorkOrderStatusEnum.Cerrado) or nameof(WorkOrderStatusEnum.Cancelado))
                {
                    await workOrderInventoryCommand.ReleaseReservationsAsync(workOrderId);
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateWorkOrderAsync(UpdateWorkOrderDTO dto, RecipeEntity? mappedRecipe)
        {
            var workOrder = await context.WorkOrders
                .Include(w => w.Recipe)
                    .ThenInclude(r => r.DetailRecipeList)
                .FirstOrDefaultAsync(w => w.Id == dto.Id);

            if (workOrder is null)
            {
                throw new NotFoundException($"No existe la orden de trabajo {dto.Id}.");
            }

            if (workOrder.Status is nameof(WorkOrderStatusEnum.Cerrado) or nameof(WorkOrderStatusEnum.Cancelado))
            {
                throw new BusinessRuleException($"No se puede modificar una orden en estado '{workOrder.Status}'.");
            }

            await unitOfWork.BeginAsync();
            try
            {
                if (dto.TaskId.HasValue)
                {
                    workOrder.TaskId = dto.TaskId.Value;
                }

                if (dto.Description is not null)
                {
                    workOrder.Description = dto.Description;
                }

                if (dto.StartDate.HasValue)
                {
                    workOrder.StartDate = dto.StartDate;
                }

                if (dto.EndDate.HasValue)
                {
                    workOrder.EndDate = dto.EndDate;
                }

                if (dto.TotalArea.HasValue)
                {
                    workOrder.TotalAreaWorked = dto.TotalArea.Value;
                }

                if (mappedRecipe is not null)
                {
                    await ApplyRecipeUpdateAsync(workOrder, mappedRecipe);
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task ApplyRecipeUpdateAsync(WorkOrderEntity workOrder, RecipeEntity newRecipe)
        {
            if (workOrder.Recipe is null)
            {
                context.Recipes.Add(newRecipe);
                workOrder.Recipe = newRecipe;
                await context.SaveChangesAsync();

                workOrder.RecipeId = newRecipe.Id;

                if (newRecipe.DetailRecipeList is { Count: > 0 })
                {
                    await workOrderInventoryCommand.RegisterReservationsAndCostsAsync(workOrder, newRecipe.DetailRecipeList);
                }

                return;
            }

            var recipe = workOrder.Recipe;
            recipe.TRV = newRecipe.TRV;
            recipe.VolumeMachine = newRecipe.VolumeMachine;
            recipe.VolumeMachineUnit = newRecipe.VolumeMachineUnit;
            recipe.MachineId = newRecipe.MachineId;

            await AdjustReservationsAsync(workOrder, newRecipe.DetailRecipeList);

            context.DetailRecipes.RemoveRange(recipe.DetailRecipeList);
            recipe.DetailRecipeList = newRecipe.DetailRecipeList.ToList();
        }

        private async Task AdjustReservationsAsync(WorkOrderEntity workOrder, ICollection<DetailRecipeEntity> newDetails)
        {
            var reservations = await context.MaterialReservations
                .Where(r => r.WorkOrderId == workOrder.Id && r.State == nameof(ReservationStateEnum.Activa))
                .ToListAsync();

            var materialIds = newDetails.Select(d => d.MaterialId)
                .Union(reservations.Select(r => r.MaterialId))
                .Distinct()
                .ToList();

            var materials = await context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var newPlanned = newDetails
                .GroupBy(d => d.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => UnitConverter.ConvertToBaseUnit(
                        x.EstimatedAmount,
                        x.EstimatedAmountUnit,
                        materials.TryGetValue(x.MaterialId, out var material) ? material.UnitOfMeasureId : null)));

            var stockRows = await context.StockByFarms
                .FromSqlRaw("SELECT * FROM StockPorFinca WITH (UPDLOCK, HOLDLOCK) WHERE IdFinca = {0}", workOrder.FarmId)
                .ToListAsync();

            var now = DateTime.Now;

            foreach (var materialId in materialIds)
            {
                var planned = newPlanned.GetValueOrDefault(materialId, 0m);
                var reservation = reservations.FirstOrDefault(r => r.MaterialId == materialId);
                var currentReserved = reservation?.ReservedAmount ?? 0m;

                if (planned > currentReserved)
                {
                    var increase = planned - currentReserved;
                    var stock = stockRows.FirstOrDefault(s => s.MaterialId == materialId);
                    var available = stock is null ? 0m : stock.StockFisico - stock.StockReservado;

                    if (increase > available)
                    {
                        throw new BusinessRuleException(
                            $"Stock insuficiente del material {materialId} en la finca {workOrder.FarmId}. Disponible {available}, requerido {increase} adicional.");
                    }

                    stock!.StockReservado += increase;

                    if (reservation is null)
                    {
                        context.MaterialReservations.Add(new MaterialReservationEntity
                        {
                            WorkOrderId = workOrder.Id,
                            MaterialId = materialId,
                            FarmId = workOrder.FarmId,
                            ReservedAmount = planned,
                            ConsumedAmount = 0,
                            State = nameof(ReservationStateEnum.Activa),
                            CreatedDate = now
                        });
                    }
                    else
                    {
                        reservation.ReservedAmount = planned;
                    }
                }
                else if (planned < currentReserved && reservation is not null)
                {
                    var decrease = currentReserved - planned;
                    var pending = reservation.ReservedAmount - reservation.ConsumedAmount;
                    var release = Math.Min(decrease, Math.Max(pending, 0m));

                    if (release > 0)
                    {
                        var stock = stockRows.FirstOrDefault(s => s.MaterialId == materialId);

                        if (stock is not null)
                        {
                            stock.StockReservado -= release;
                        }
                    }

                    reservation.ReservedAmount = planned;

                    if (reservation.ConsumedAmount >= planned)
                    {
                        reservation.State = nameof(ReservationStateEnum.Consumida);
                    }
                }
            }
        }

        private async Task ValidateCancellableAsync(int workOrderId)
        {
            var hasActivities = await context.DetailWorkOrders
                .AnyAsync(d => d.WorkOrderId == workOrderId);

            if (hasActivities)
            {
                throw new BusinessRuleException("No se puede cancelar una orden con actividades registradas.");
            }

            var hasConsumptions = await context.Consumptions
                .AnyAsync(c => c.WorkOrderId == workOrderId && (c.ConsumedAmount > 0 || c.AppliedAmount > 0));

            if (hasConsumptions)
            {
                throw new BusinessRuleException("No se puede cancelar una orden con consumos registrados.");
            }
        }
    }
}
