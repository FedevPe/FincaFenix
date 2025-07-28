using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface ITaskRepository
    {
        Task<TaskEntity> GetTaskById(int id);
        Task<IEnumerable<TaskEntity>> GetAllTasks();
        Task<bool> Exists(int id);
    }
}
