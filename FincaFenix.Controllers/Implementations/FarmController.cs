using Microsoft.AspNetCore.Mvc;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Farms;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

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
