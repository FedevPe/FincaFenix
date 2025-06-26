using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Machine;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/(controller)")]
    public class MachineController(
        IMachineInputPort interactor,
        IMachineOutputPort presenter) : IMachineController
    {
        [HttpGet("getmachinelist")]
        public async Task<IEnumerable<MachineRecipeDTO>> GetMachines()
        {
            await interactor.GetMachineList();
            return presenter.Machines;
        }
    }
}
