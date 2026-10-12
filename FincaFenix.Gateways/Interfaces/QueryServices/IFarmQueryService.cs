using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IFarmQueryService
    {
        Task<IEnumerable<FarmEntity>> GetFarmList();
        Task<(IEnumerable<FarmEntity> Farms, int TotalCount)> GetFarmListPaged(FarmFilterDTO filter);
        Task<FarmEntity> GetFarmById(int farmId);
        Task<bool> Exists(int id);
    }
}
