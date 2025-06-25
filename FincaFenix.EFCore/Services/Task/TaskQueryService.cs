using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaFenix.EFCore.Services.Task
{
    public class TaskQueryService : FincaFenixContext, ITaskQueryService
    {
        public async Task<IEnumerable<TaskEntity>> GetTaks()
        {
            return await Tasks.ToListAsync();
        }
    }
}
