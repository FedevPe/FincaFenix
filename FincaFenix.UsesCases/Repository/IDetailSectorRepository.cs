using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IDetailSectorRepository
    {
        Task<IEnumerable<DetailSectorFarmEntity>> GetAllSectorsByFarmId(int farmId);
        Task<IEnumerable<DetailSectorFarmEntity>> GetAllSectorsByOrderId(int orderId);
    }
}
