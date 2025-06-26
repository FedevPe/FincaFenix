using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class TaskQueryService : FincaFenixContext, ITaskQueryService
    {
        public async Task<IEnumerable<TaskEntity>> GetTaks()
        {
            return await Tasks.OrderBy(t => t.Description).ToListAsync();
        }
    }
}
