using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Machine;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.MACHINE_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class MachineController(ISender mediator) : ControllerBase,IMachineController
    {
        [HttpGet("getmachinelist")]
        [ProducesResponseType(typeof(IEnumerable<MachineRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MachineRecipeDTO>> GetMachines()
        {
            return await mediator.Send(new GetMachineListQuery());
        }
    }
}
