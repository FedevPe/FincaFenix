using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface ITaskQueryService
    {
        Task<IEnumerable<TaskEntity>> GetTaskList();
        Task<TaskEntity> GetTaskById(int id);
    }
}
