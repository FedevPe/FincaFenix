using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IUnitOfMeasureQueryService
    {
        Task<IEnumerable<UnitOfMeasureEntity>> GetList();
        Task<UnitOfMeasureEntity> GetById(int id);
        Task<bool> Exists(int id);
    }
}
