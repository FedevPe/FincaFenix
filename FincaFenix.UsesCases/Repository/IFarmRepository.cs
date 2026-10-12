using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IFarmRepository
    {
        Task<IEnumerable<FarmEntity>> GetListFarm();
        Task<(IEnumerable<FarmEntity> Farms, int TotalCount)> GetFarmListPaged(FarmFilterDTO filter);
        Task<FarmEntity> GetFarmById(int farmId);
        Task<bool> Exists(int id);
        Task<FarmEntity> Add(FarmEntity farm);
        Task<FarmEntity> Update(FarmEntity farm);
        Task Delete(int id);
        Task<FarmEntity> Reactivate(int id);
    }
}