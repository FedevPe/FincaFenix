using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(
        ITaskInputPort interactor,
        ITaskOuputPort presenter) : ITaskController
    {

        public Task<TaskDTO> DisplayTaskById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("GetTaskList")]
        public async Task<IEnumerable<TaskDTO>> DisplayTaskList()
        {
            await interactor.GetListTask();
            return presenter.TaskList;
        }
    }
}
