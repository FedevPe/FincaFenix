using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Controllers
{
    public interface ISectorController
    {
        Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByFarmId(int farmId);
        Task<IEnumerable<DetailSectorFarmDTO>> DisplayListSectorByOrderId(int orderId);
    }
}
