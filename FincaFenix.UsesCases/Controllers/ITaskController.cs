using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.TaskDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ITaskController
    {
        Task<IEnumerable<TaskDTO>> GetTaskList(bool includeDeleted);
        Task<PagedResult<TaskDTO>> GetTaskListPaged(TaskFilterDTO filter);
        Task<TaskDTO> GetTaskById(int id);
        Task<TaskDTO> CreateTask(SaveTaskDTO dto);
        Task<TaskDTO> UpdateTask(SaveTaskDTO dto);
        Task<bool> DeleteTask(int id);
        Task<TaskDTO> ReactivateTask(int id);
    }
}
