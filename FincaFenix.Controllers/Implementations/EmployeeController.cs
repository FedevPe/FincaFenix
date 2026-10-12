using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Employee;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(ISender mediator) : ControllerBase, IEmployeeController
    {
        [HttpGet("{farmId}/employees")]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_READ)]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId)
        {
            return await mediator.Send(new GetEmployeeListQuery(farmId));
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_READ)]
        [ProducesResponseType(typeof(PagedResult<EmployeeDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<EmployeeDTO>> GetEmployeeListPaged([FromQuery] EmployeeFilterDTO filter)
        {
            return await mediator.Send(new GetEmployeeListPagedQuery(filter));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_READ)]
        [ProducesResponseType(typeof(EmployeeDTO),(int)HttpStatusCode.OK)]
        public async Task<EmployeeDTO> GetEmployeeById(int id)
        {
            return await mediator.Send(new GetEmployeeByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_CREATE)]
        [ProducesResponseType(typeof(EmployeeDTO),(int)HttpStatusCode.Created)]
        public async Task<EmployeeDTO> CreateEmployee([FromBody] SaveEmployeeDTO dto)
        {
            return await mediator.Send(new CreateEmployeeCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_UPDATE)]
        [ProducesResponseType(typeof(EmployeeDTO),(int)HttpStatusCode.OK)]
        public async Task<EmployeeDTO> UpdateEmployee([FromBody] SaveEmployeeDTO dto)
        {
            return await mediator.Send(new UpdateEmployeeCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteEmployee(int id)
        {
            await mediator.Send(new DeleteEmployeeCommand(id));
            return true;
        }

        [HttpPut("ReactivateEmployee/{id}")]
        [Authorize(Policy = PolicyMaster.EMPLOYEE_UPDATE)]
        [ProducesResponseType(typeof(EmployeeDTO),(int)HttpStatusCode.OK)]
        public async Task<EmployeeDTO> ReactivateEmployee(int id)
        {
            return await mediator.Send(new ReactivateEmployeeCommand(id));
        }
    }
}