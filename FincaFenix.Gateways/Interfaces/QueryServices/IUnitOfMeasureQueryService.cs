using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IUnitOfMeasureQueryService
    {
        Task<IEnumerable<UnitOfMeasureEntity>> GetList();
        Task<IEnumerable<UnitOfMeasureEntity>> GetAll(bool includeDeleted = false);
        Task<UnitOfMeasureEntity> GetById(int id);
        Task<bool> Exists(int id);
    }
}
