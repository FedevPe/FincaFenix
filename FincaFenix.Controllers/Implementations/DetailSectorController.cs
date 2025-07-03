using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Sectors;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetailSectorController(
        IDetailSectorInputPort interactor,
        IDetailSectorOutputPort presenter) : IDetailSectorController
    {
        [HttpGet("farm/{farmId}/sectors")]
        public async Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByFarmId(int farmId)
        {
            await interactor.GetListSectorsByIdFarm(farmId);
            return presenter.SectorList;
        }
        [HttpGet("order/{orderId}/sectors")]
        public async Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByOrderId(int orderId)
        {
            await interactor.GetSectorsByOrderId(orderId);
            return presenter.SectorList;
        }
    }
}
