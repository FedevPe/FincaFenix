using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Tasks
{
    public interface ITaskOuputPort
    {
        Task Handle(TaskDTO task);
    }
}
