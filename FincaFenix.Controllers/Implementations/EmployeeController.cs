using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(
        IEmployeeInputPort interactor,
        IEmployeeOutputPort presenter) : IEmployeeController
    {
        [HttpGet("{farmId}/employees")]
        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId)
        {
            await interactor.GetEmployeeList(farmId);
            return presenter.EmployeeList;
        }
    }
}
