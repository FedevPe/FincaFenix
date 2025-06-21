using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Tasks
{
    public interface ITaskInputPort
    {
        Task GetListTask();
        Task GetTaskById(int id);
    }
}
