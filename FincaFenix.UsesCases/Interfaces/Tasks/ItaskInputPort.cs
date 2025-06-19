using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Tasks
{
    public interface ITaskInputPort
    {
        Task Handle(TaskDTO task);
        Task GetListTask();
        Task GetTaskById(int id);
    }
}
