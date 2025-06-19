using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface ITaskRepository
    {
        Task<TaskEntity> GetTaskById();
        Task<IEnumerable<TaskEntity>> GetAllTasks();
    }
}
