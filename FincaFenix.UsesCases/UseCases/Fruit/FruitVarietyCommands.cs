using AutoMapper;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Fruit;

public record CreateFruitVarietyCommand(SaveFruitVarietyDTO Dto) : IRequest<FruitVarietyDTO>;
public record UpdateFruitVarietyCommand(SaveFruitVarietyDTO Dto) : IRequest<FruitVarietyDTO>;
public record DeleteFruitVarietyCommand(int Id) : IRequest<Unit>;

public class CreateFruitVarietyHandler(IFruitVarietyRepository repository, IMapper mapper) : IRequestHandler<CreateFruitVarietyCommand, FruitVarietyDTO>
{
    public async Task<FruitVarietyDTO> Handle(CreateFruitVarietyCommand request, CancellationToken cancellationToken)
    {
        var variety = mapper.Map<FruitVarietyEntity>(request.Dto);
        var created = await repository.Add(variety);
        return mapper.Map<FruitVarietyDTO>(created);
    }
}

public class UpdateFruitVarietyHandler(IFruitVarietyRepository repository, IMapper mapper) : IRequestHandler<UpdateFruitVarietyCommand, FruitVarietyDTO>
{
    public async Task<FruitVarietyDTO> Handle(UpdateFruitVarietyCommand request, CancellationToken cancellationToken)
    {
        var variety = mapper.Map<FruitVarietyEntity>(request.Dto);
        var updated = await repository.Update(variety);
        return mapper.Map<FruitVarietyDTO>(updated);
    }
}

public class DeleteFruitVarietyHandler(IFruitVarietyRepository repository) : IRequestHandler<DeleteFruitVarietyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFruitVarietyCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}
