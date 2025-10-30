using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ITaskController
    {
        Task<IEnumerable<TaskDTO>> GetTaskList();
        Task<TaskDTO> GetTaskById(int id);
    }
}
