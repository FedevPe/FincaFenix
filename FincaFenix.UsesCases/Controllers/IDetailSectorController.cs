using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IDetailSectorController
    {
        Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByFarmId(int farmId);
        Task<IEnumerable<DetailSectorFarmDTO>> GetListSectorByOrderId(int orderId);
    }
}
