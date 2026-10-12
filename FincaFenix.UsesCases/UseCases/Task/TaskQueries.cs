using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.TaskDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Task;

public record GetTaskListQuery(bool IncludeDeleted = false) : IRequest<IEnumerable<TaskDTO>>;
public record GetTaskListPagedQuery(TaskFilterDTO Filter) : IRequest<PagedResult<TaskDTO>>;
public record GetTaskByIdQuery(int Id) : IRequest<TaskDTO>;

public class GetTaskListHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<GetTaskListQuery, IEnumerable<TaskDTO>>
{
    public async Task<IEnumerable<TaskDTO>> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
    {
        var tasks = await repository.GetAllTasks(request.IncludeDeleted);
        return mapper.Map<IEnumerable<TaskDTO>>(tasks);
    }
}

public class GetTaskListPagedHandler(ITaskRepository repository, IMapper mapper) : IRequestHandler<GetTaskListPagedQuery, PagedResult<TaskDTO>>
{
    public async Task<PagedResult<TaskDTO>> Handle(GetTaskListPagedQuery request, CancellationToken cancellationToken)
    {
        var (tasks, totalCount) = await repository.GetTaskListPaged(request.Filter);

        return new PagedResult<TaskDTO>
        {
            Items = mapper.Map<IEnumerable<TaskDTO>>(tasks),
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
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
