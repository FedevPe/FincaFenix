using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IFarmRepository
    {
        Task<IEnumerable<FarmEntity>> GetListFarm();
        Task<FarmEntity> GetFarmById(int farmId);
    }
}
