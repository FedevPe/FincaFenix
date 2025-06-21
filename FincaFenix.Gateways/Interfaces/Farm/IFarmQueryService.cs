using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Farm
{
    public interface IFarmQueryService
    {
        Task<IEnumerable<FarmEntity>> GetFarmList();
    }
}
