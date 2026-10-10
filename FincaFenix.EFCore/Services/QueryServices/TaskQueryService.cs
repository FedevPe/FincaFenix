using FincaFenix.EFCore.Context;
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
    }
}
