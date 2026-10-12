using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.TaskDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(ISender mediator) : ControllerBase, ITaskController
    {
        [HttpGet("getTaskById/{id}")]
        [Authorize(Policy = PolicyMaster.TASK_READ)]
        [ProducesResponseType(typeof(TaskDTO),(int)HttpStatusCode.OK)]
        public async Task<TaskDTO> GetTaskById(int id)
        {
            return await mediator.Send(new GetTaskByIdQuery(id));
        }

        [HttpGet("GetTaskList")]
        [Authorize(Policy = PolicyMaster.TASK_READ)]
        [ProducesResponseType(typeof(IEnumerable<TaskDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<TaskDTO>> GetTaskList(bool includeDeleted = false)
        {
            return await mediator.Send(new GetTaskListQuery(includeDeleted));
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.TASK_READ)]
        [ProducesResponseType(typeof(PagedResult<TaskDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<TaskDTO>> GetTaskListPaged([FromQuery] TaskFilterDTO filter)
        {
            return await mediator.Send(new GetTaskListPagedQuery(filter));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.TASK_CREATE)]
        [ProducesResponseType(typeof(TaskDTO),(int)HttpStatusCode.Created)]
        public async Task<TaskDTO> CreateTask([FromBody] SaveTaskDTO dto)
        {
            return await mediator.Send(new CreateTaskCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.TASK_UPDATE)]
        [ProducesResponseType(typeof(TaskDTO),(int)HttpStatusCode.OK)]
        public async Task<TaskDTO> UpdateTask([FromBody] SaveTaskDTO dto)
        {
            return await mediator.Send(new UpdateTaskCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.TASK_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteTask(int id)
        {
            await mediator.Send(new DeleteTaskCommand(id));
            return true;
        }

        [HttpPut("ReactivateTask/{id}")]
        [Authorize(Policy = PolicyMaster.TASK_UPDATE)]
        [ProducesResponseType(typeof(TaskDTO),(int)HttpStatusCode.OK)]
        public async Task<TaskDTO> ReactivateTask(int id)
        {
            return await mediator.Send(new ReactivateTaskCommand(id));
        }
    }
}
