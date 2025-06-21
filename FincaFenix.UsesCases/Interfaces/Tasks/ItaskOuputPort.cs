using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Tasks
{
    public interface ITaskOuputPort
    {
        public List<TaskDTO> TaskList { get; }
        Task Handle(TaskEntity task);
        Task HandleList(IEnumerable<TaskEntity> taskList);
    }
}
