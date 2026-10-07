using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Task;

public record GetTaskListQuery : IRequest<IEnumerable<TaskDTO>>;
public record GetTaskByIdQuery(int Id) : IRequest<TaskDTO>;

public class GetTaskListHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<GetTaskListQuery, IEnumerable<TaskDTO>>
{
    public async Task<IEnumerable<TaskDTO>> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
    {
        var tasks = await repository.GetAllTasks();
        return mapper.Map<IEnumerable<TaskDTO>>(tasks);
    }
}

public class GetTaskByIdHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<GetTaskByIdQuery, TaskDTO>
{
    public async Task<TaskDTO> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await repository.GetTaskById(request.Id);
        return mapper.Map<TaskDTO>(task);
    }
}
