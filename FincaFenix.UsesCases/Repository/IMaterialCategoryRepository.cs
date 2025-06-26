using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialCategoryRepository
    {
        Task<IEnumerable<MaterialCategoryEntity>> GetAllCategories();
    }
}
