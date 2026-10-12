using AutoMapper;
using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Farm;

public record CreateFarmCommand(SaveFarmDTO Dto) : IRequest<FarmDTO>;
public record UpdateFarmCommand(SaveFarmDTO Dto) : IRequest<FarmDTO>;
public record DeleteFarmCommand(int Id) : IRequest<Unit>;
public record ReactivateFarmCommand(int Id) : IRequest<FarmDTO>;

public class CreateFarmHandler(IFarmRepository repository, IMapper mapper) : IRequestHandler<CreateFarmCommand, FarmDTO>
{
    public async Task<FarmDTO> Handle(CreateFarmCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<FarmEntity>(request.Dto);
        var created = await repository.Add(entity);
        return mapper.Map<FarmDTO>(created);
    }
}

public class UpdateFarmHandler(IFarmRepository repository, IMapper mapper) : IRequestHandler<UpdateFarmCommand, FarmDTO>
{
    public async Task<FarmDTO> Handle(UpdateFarmCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<FarmEntity>(request.Dto);
        var updated = await repository.Update(entity);
        return mapper.Map<FarmDTO>(updated);
    }
}

public class DeleteFarmHandler(IFarmRepository repository) : IRequestHandler<DeleteFarmCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}

public class ReactivateFarmHandler(IFarmRepository repository, IMapper mapper) : IRequestHandler<ReactivateFarmCommand, FarmDTO>
{
    public async Task<FarmDTO> Handle(ReactivateFarmCommand request, CancellationToken cancellationToken)
    {
        var farm = await repository.Reactivate(request.Id);
        return mapper.Map<FarmDTO>(farm);
    }
}