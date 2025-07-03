using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ITaskController
    {
        Task<IEnumerable<TaskDTO>> DisplayTaskList();
        Task<TaskDTO> GetTaskById(int id);
    }
}
