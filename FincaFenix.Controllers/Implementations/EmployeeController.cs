using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Employee;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.EMPLOYEE_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(ISender mediator) : ControllerBase, IEmployeeController
    {
        [HttpGet("{farmId}/employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId)
        {
            return await mediator.Send(new GetEmployeeListQuery(farmId));
        }
    }
}
