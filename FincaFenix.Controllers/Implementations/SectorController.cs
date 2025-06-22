using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Sectors;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectorController(
        ISectorsInputPort interactor,
        ISectorOutputPort presenter) : ISectorController
    {
        [HttpGet("GetListSectorByFarmId")]        
        public async Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByFarmId(int farmId)
        {
            await interactor.GetListSectorsByIdFarm(farmId);
            return presenter.SectorList;
        }

        public async Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByOrderId(int orderId)
        {
            await interactor.GetListSectorsByIdFarm(orderId);
            return presenter.SectorList;
        }
    }
}
