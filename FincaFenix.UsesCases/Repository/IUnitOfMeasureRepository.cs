using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IUnitOfMeasureRepository
    {
        Task<IEnumerable<UnitOfMeasureEntity>> GetList();
        Task<UnitOfMeasureEntity> GetById(int id);
        Task<bool> Exists(int id);
        Task<UnitOfMeasureEntity> Add(UnitOfMeasureEntity unit);
        Task<UnitOfMeasureEntity> Update(UnitOfMeasureEntity unit);
        Task Delete(int id);
    }
}
