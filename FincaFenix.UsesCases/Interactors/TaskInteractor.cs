using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.Tasks;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class TaskInteractor(
        ITaskOutputPort presenter,
        ITaskRepository repository) : ITaskInputPort
    {
        public async Task GetListTask()
        {
            await presenter.HandleList(await repository.GetAllTasks());
        }

        public async Task GetTaskById(int id)
        {
            await presenter.Handle(await repository.GetTaskById(id));
        }
    }
}
