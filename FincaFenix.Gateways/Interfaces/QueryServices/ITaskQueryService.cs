using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface ITaskQueryService
    {
        Task<IEnumerable<TaskEntity>> GetTaskList(bool includeDeleted = false);
        Task<TaskEntity> GetTaskById(int id);
        Task<bool> Exists(int id);
    }
}
