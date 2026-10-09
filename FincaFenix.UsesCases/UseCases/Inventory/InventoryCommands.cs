using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository.Inventory;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Inventory;

public record RegisterMovementCommand(RegisterMovementDTO Dto) : IRequest<MovementResultDTO>;

public class RegisterMovementHandler(IInventoryRepository repository) : IRequestHandler<RegisterMovementCommand, MovementResultDTO>
{
    public Task<MovementResultDTO> Handle(RegisterMovementCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var movement = new InventoryMovementEntity
        {
            MaterialId = dto.MaterialId,
            FarmId = dto.FarmId,
            MovementType = dto.MovementType,
            Amount = dto.Amount,
            UnitCost = dto.UnitCost,
            CurrencyId = dto.CurrencyId,
            Observations = dto.Observations,
            UserId = dto.UserId,
            Origin = InventoryOriginEnum.Manual.ToString()
        };

        return repository.RegisterMovementAsync(movement, dto.StockMinimum);
    }
}

public record RegisterConsumptionCommand(RegisterConsumptionDTO Dto) : IRequest<ConsumptionResultDTO>;

public class RegisterConsumptionHandler(IInventoryRepository repository) : IRequestHandler<RegisterConsumptionCommand, ConsumptionResultDTO>
{
    public Task<ConsumptionResultDTO> Handle(RegisterConsumptionCommand request, CancellationToken cancellationToken)
    {
        return repository.RegisterConsumptionAsync(request.Dto);
    }
}
