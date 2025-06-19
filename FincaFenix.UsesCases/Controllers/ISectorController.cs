using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ISectorController
    {
        Task<IEnumerable<SectorDTO>> DisplayListSectorByFarmId(int farmId);
        Task<IEnumerable<SectorDTO>> DisplayListSectorByOrderId(int orderId);
    }
}
