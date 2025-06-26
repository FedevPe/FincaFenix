using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IFarmQueryService
    {
        Task<IEnumerable<FarmEntity>> GetFarmList();
    }
}
