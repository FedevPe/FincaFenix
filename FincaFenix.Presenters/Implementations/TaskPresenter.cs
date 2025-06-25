using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Tasks;

namespace FincaFenix.Presenters.Implementations
{
    public class TaskPresenter : ITaskOutputPort
    {
        public List<TaskDTO>? TaskList { get; private set; }

        public Task Handle(TaskEntity task)
        {
            throw new NotImplementedException();
        }

        public Task HandleList(IEnumerable<TaskEntity> taskList)
        {
            TaskList = taskList.Select(t => new TaskDTO
            {
                Id = t.Id,
                Description = t.Description
            }).ToList();
            return Task.CompletedTask;
        }
    }
}
