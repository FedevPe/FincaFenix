using Microsoft.AspNetCore.Mvc;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmController(
        IFarmInputPort interactor,
        IFarmOutputPort presenter) : IFarmController
    {
        [HttpGet("GetListFarm")]
        public async Task<IEnumerable<FarmDTO>> DisplayListFarm()
        {
            await interactor.GetListFarm();
            return presenter.FarmList;
        }
    }
}
