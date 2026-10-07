using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.TASK_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(ISender mediator) : ControllerBase, ITaskController
    {
        [HttpGet("getTaskById/{id}")]
        [ProducesResponseType(typeof(MaterialRecipeDTO),(int)HttpStatusCode.OK)]
        public async Task<TaskDTO> GetTaskById(int id)
        {
            return await mediator.Send(new GetTaskByIdQuery(id));
        }

        [HttpGet("GetTaskList")]
        [ProducesResponseType(typeof(IEnumerable<TaskDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<TaskDTO>> GetTaskList()
        {
            return await mediator.Send(new GetTaskListQuery());
        }
    }
}
