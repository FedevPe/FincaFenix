using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Inventory;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Inventory
{
    public class InventoryCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IInventoryCommandService
    {
        public async Task<MovementResultDTO> RegisterMovementAsync(InventoryMovementEntity movement, decimal? stockMinimum)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var material = await context.Materials
                    .FirstOrDefaultAsync(m => m.Id == movement.MaterialId);

                if (material is null)
                    throw new NotFoundException($"No se encontró el material con Id {movement.MaterialId}.");

                var farmExists = await context.Farms.AnyAsync(f => f.Id == movement.FarmId);

                if (!farmExists)
                    throw new NotFoundException($"No se encontró la finca con Id {movement.FarmId}.");

                var currencyExists = await context.Currencies.AnyAsync(c => c.Id == movement.CurrencyId);

                if (!currencyExists)
                    throw new NotFoundException($"No se encontró la divisa con Id {movement.CurrencyId}.");

                var stock = await context.StockByFarms
                    .FirstOrDefaultAsync(s => s.MaterialId == movement.MaterialId && s.FarmId == movement.FarmId);

                if (stock is null)
                {
                    stock = new StockByFarmEntity
                    {
                        MaterialId = movement.MaterialId,
                        FarmId = movement.FarmId,
                        StockFisico = 0,
                        StockReservado = 0,
                        StockMinimo = 0
                    };

                    await context.StockByFarms.AddAsync(stock);
                }

                var previousStock = stock.StockFisico;
                var resultingStock = previousStock + GetSignedAmount(movement.MovementType, movement.Amount);

                if (resultingStock < 0)
                    throw new BusinessRuleException(
                        $"Stock insuficiente. Stock actual {previousStock}, no se puede aplicar un movimiento {movement.MovementType} de {movement.Amount}.");

                stock.StockFisico = resultingStock;

                if (stockMinimum.HasValue)
                    stock.StockMinimo = stockMinimum.Value;

                if (movement.UnitCost.HasValue)
                {
                    material.ReferenceCost = movement.UnitCost.Value;
                    material.CurrencyId = movement.CurrencyId;
                }

                movement.PreviousStock = previousStock;
                movement.ResultingStock = resultingStock;
                movement.Date = DateTime.Now;
                movement.TotalCost = movement.UnitCost.HasValue
                    ? movement.UnitCost.Value * Math.Abs(movement.Amount)
                    : null;

                await context.InventoryMovements.AddAsync(movement);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return new MovementResultDTO
                {
                    MovementId = movement.Id,
                    PreviousStock = previousStock,
                    ResultingStock = resultingStock
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private static decimal GetSignedAmount(string movementType, decimal amount)
        {
            return movementType switch
            {
                nameof(InventoryMovementTypeEnum.Ingreso) => amount,
                nameof(InventoryMovementTypeEnum.AjustePositivo) => amount,
                nameof(InventoryMovementTypeEnum.AjusteNegativo) => -amount,
                nameof(InventoryMovementTypeEnum.SalidaConsumo) => -amount,
                nameof(InventoryMovementTypeEnum.SalidaManual) => -amount,
                _ => throw new BusinessRuleException($"Tipo de movimiento desconocido: {movementType}.")
            };
        }
    }
}
