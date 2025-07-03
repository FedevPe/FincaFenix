using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class TaskRepository(ITaskQueryService queryService) : ITaskRepository
    {
        public async Task<IEnumerable<TaskEntity>> GetAllTasks()
        {
            return await queryService.GetTaskList();
        }
        
        public Task<TaskEntity> GetTaskById(int id)
        {
            return queryService.GetTaskById(id);
        }
    }
}
