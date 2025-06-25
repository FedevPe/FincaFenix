using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Sector
{
    public interface IDetailSectorQueryService
    {
        Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId);

    }
}
