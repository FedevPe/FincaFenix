using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Task
{
    public interface ITaskQueryService
    {
        Task<IEnumerable<TaskEntity>> GetTaks();
    }
}
