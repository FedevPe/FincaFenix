using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Task;

public record CreateTaskCommand(SaveTaskDTO Dto) : IRequest<TaskDTO>;
public record UpdateTaskCommand(SaveTaskDTO Dto) : IRequest<TaskDTO>;
public record DeleteTaskCommand(int Id) : IRequest<Unit>;

public class CreateTaskHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<CreateTaskCommand, TaskDTO>
{
    public async Task<TaskDTO> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<TaskEntity>(request.Dto);
        var created = await repository.Add(entity);
        return mapper.Map<TaskDTO>(created);
    }
}

public class UpdateTaskHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<UpdateTaskCommand, TaskDTO>
{
    public async Task<TaskDTO> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<TaskEntity>(request.Dto);
        var updated = await repository.Update(entity);
        return mapper.Map<TaskDTO>(updated);
    }
}

public class DeleteTaskHandler(ITaskRepository repository) : IRequestHandler<DeleteTaskCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}
