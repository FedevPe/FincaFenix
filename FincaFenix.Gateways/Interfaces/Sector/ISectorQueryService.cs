using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Sector
{
    public interface ISectorQueryService
    {
        Task<IEnumerable<SectorEntity>> GetSectorListByFarmId(int farmId);
        Task<IEnumerable<SectorEntity>> GetSectorListByOrderId(int orderId);

    }
}
