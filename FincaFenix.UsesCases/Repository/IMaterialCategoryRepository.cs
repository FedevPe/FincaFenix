using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialCategoryRepository
    {
        Task<IEnumerable<MaterialCategoryEntity>> GetAllCategories();
        Task<bool> Exists(int id);
    }
}
