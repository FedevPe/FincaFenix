using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Sectors;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetailSectorController(
        ISectorsInputPort interactor,
        ISectorOutputPort presenter) : ISectorController
    {
        [HttpGet("{farmId}/sectors")]
        public async Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByFarmId(int farmId)
        {
            await interactor.GetListSectorsByIdFarm(farmId);
            return presenter.SectorList;
        }

        public async Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByOrderId(int orderId)
        {
            await interactor.GetSectorsByOrderId(orderId);
            return presenter.SectorList;
        }
    }
}
