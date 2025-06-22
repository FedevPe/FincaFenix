using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Sector
{
    public interface ISectorQueryService
    {
        Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId);
        Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByOrderId(int orderId);

    }
}
