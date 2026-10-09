using AutoMapper;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.UnitOfMeasure;

public record GetUnitOfMeasureListQuery : IRequest<IEnumerable<UnitOfMeasureDTO>>;
public record GetUnitOfMeasureByIdQuery(int Id) : IRequest<UnitOfMeasureDTO>;
public record CreateUnitOfMeasureCommand(SaveUnitOfMeasureDTO Dto) : IRequest<UnitOfMeasureDTO>;
public record UpdateUnitOfMeasureCommand(SaveUnitOfMeasureDTO Dto) : IRequest<UnitOfMeasureDTO>;
public record DeleteUnitOfMeasureCommand(int Id) : IRequest<Unit>;

public class GetUnitOfMeasureListHandler(IUnitOfMeasureRepository repository, IMapper mapper) : IRequestHandler<GetUnitOfMeasureListQuery, IEnumerable<UnitOfMeasureDTO>>
{
    public async Task<IEnumerable<UnitOfMeasureDTO>> Handle(GetUnitOfMeasureListQuery request, CancellationToken cancellationToken)
    {
        var units = await repository.GetList();
        return mapper.Map<IEnumerable<UnitOfMeasureDTO>>(units);
    }
}

public class GetUnitOfMeasureByIdHandler(IUnitOfMeasureRepository repository, IMapper mapper) : IRequestHandler<GetUnitOfMeasureByIdQuery, UnitOfMeasureDTO>
{
    public async Task<UnitOfMeasureDTO> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetById(request.Id);

        if (unit is null)
            throw new NotFoundException($"No se encontró la unidad de medida con Id {request.Id}.");

        return mapper.Map<UnitOfMeasureDTO>(unit);
    }
}

public class CreateUnitOfMeasureHandler(IUnitOfMeasureRepository repository, IMapper mapper) : IRequestHandler<CreateUnitOfMeasureCommand, UnitOfMeasureDTO>
{
    public async Task<UnitOfMeasureDTO> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<UnitOfMeasureEntity>(request.Dto);
        var created = await repository.Add(entity);
        return mapper.Map<UnitOfMeasureDTO>(created);
    }
}

public class UpdateUnitOfMeasureHandler(IUnitOfMeasureRepository repository, IMapper mapper) : IRequestHandler<UpdateUnitOfMeasureCommand, UnitOfMeasureDTO>
{
    public async Task<UnitOfMeasureDTO> Handle(UpdateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<UnitOfMeasureEntity>(request.Dto);
        var updated = await repository.Update(entity);
        return mapper.Map<UnitOfMeasureDTO>(updated);
    }
}

public class DeleteUnitOfMeasureHandler(IUnitOfMeasureRepository repository) : IRequestHandler<DeleteUnitOfMeasureCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}
