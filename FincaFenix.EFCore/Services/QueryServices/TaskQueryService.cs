using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.TaskDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class TaskQueryService(
        FincaFenixContext context) : ITaskQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await context.Tasks.AnyAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<TaskEntity> GetTaskById(int taskId)
        {
            return await context.Tasks.Where(t => t.Id == taskId).FirstOrDefaultAsync() ?? throw new NotFoundException($"Task with ID {taskId} not found.");
        }

        public async Task<IEnumerable<TaskEntity>> GetTaskList(bool includeDeleted = false)
        {
            return await context.Tasks
                .Where(t => includeDeleted || !t.IsDeleted)
                .OrderBy(t => t.Description)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetTaskListPaged(TaskFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Tasks.AsQueryable();

            if (!filter.IncludeDeleted)
                baseQuery = baseQuery.Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(t => t.Description.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var tasks = await baseQuery
                .OrderBy(t => t.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (tasks, totalCount);
        }
    }
}
