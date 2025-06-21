using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Task;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class TaskRepository(ITaskQueryService queryService) : ITaskRepository
    {
        public async Task<IEnumerable<TaskEntity>> GetAllTasks()
        {
            return await queryService.GetTaks();
        }
        
        public Task<TaskEntity> GetTaskById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
