using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class TaskQueryService : FincaFenixContext, ITaskQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await Tasks.AnyAsync(t => t.Id == id);
        }

        public async Task<TaskEntity> GetTaskById(int taskId)
        {
            return await Tasks.Where(t => t.Id == taskId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        public async Task<IEnumerable<TaskEntity>> GetTaskList()
        {
            return await Tasks.OrderBy(t => t.Description).ToListAsync();
        }
    }
}
