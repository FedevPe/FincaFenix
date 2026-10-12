using AutoMapper;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Fruit;

public record CreateFruitCommand(SaveFruitDTO Dto) : IRequest<FruitDTO>;
public record UpdateFruitCommand(SaveFruitDTO Dto) : IRequest<FruitDTO>;
public record DeleteFruitCommand(int Id) : IRequest<Unit>;

public class CreateFruitHandler(IFruitRepository repository, IMapper mapper) : IRequestHandler<CreateFruitCommand, FruitDTO>
{
    public async Task<FruitDTO> Handle(CreateFruitCommand request, CancellationToken cancellationToken)
    {
        var fruit = mapper.Map<FruitEntity>(request.Dto);
        var created = await repository.Add(fruit);
        return mapper.Map<FruitDTO>(created);
    }
}

public class UpdateFruitHandler(IFruitRepository repository, IMapper mapper) : IRequestHandler<UpdateFruitCommand, FruitDTO>
{
    public async Task<FruitDTO> Handle(UpdateFruitCommand request, CancellationToken cancellationToken)
    {
        var fruit = mapper.Map<FruitEntity>(request.Dto);
        var updated = await repository.Update(fruit);
        return mapper.Map<FruitDTO>(updated);
    }
}

public class DeleteFruitHandler(IFruitRepository repository) : IRequestHandler<DeleteFruitCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFruitCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}
