using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IDetailSectorQueryService
    {
        Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId);
        Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByOrderId(int orderId);
        Task<bool> Exists(int id);

    }
}
