using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Tasks
{
    public interface ITaskCommandService
    {
        Task<TaskEntity> CreateAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task DeleteAsync(int id);
    }
}
