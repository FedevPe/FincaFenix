using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Tasks;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class TaskRepository(
        ITaskQueryService queryService,
        ITaskCommandService commandService) : ITaskRepository
    {
        public async Task<bool> Exists(int id)
        {
            return await queryService.Exists(id);
        }

        public async Task<IEnumerable<TaskEntity>> GetAllTasks(bool includeDeleted = false)
        {
            return await queryService.GetTaskList(includeDeleted);
        }
        
        public Task<TaskEntity> GetTaskById(int id)
        {
            return queryService.GetTaskById(id);
        }

        public async Task<TaskEntity> Add(TaskEntity task)
        {
            return await commandService.CreateAsync(task);
        }

        public async Task<TaskEntity> Update(TaskEntity task)
        {
            return await commandService.UpdateAsync(task);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }
    }
}
