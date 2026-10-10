using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface ITaskRepository
    {
        Task<TaskEntity> GetTaskById(int id);
        Task<IEnumerable<TaskEntity>> GetAllTasks(bool includeDeleted = false);
        Task<bool> Exists(int id);
        Task<TaskEntity> Add(TaskEntity task);
        Task<TaskEntity> Update(TaskEntity task);
        Task Delete(int id);
    }
}
