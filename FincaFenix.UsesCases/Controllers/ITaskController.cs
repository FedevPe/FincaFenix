using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ITaskController
    {
        Task<IEnumerable<TaskDTO>> GetTaskList(bool includeDeleted);
        Task<TaskDTO> GetTaskById(int id);
        Task<TaskDTO> CreateTask(SaveTaskDTO dto);
        Task<TaskDTO> UpdateTask(SaveTaskDTO dto);
        Task<bool> DeleteTask(int id);
    }
}
