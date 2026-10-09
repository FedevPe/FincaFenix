using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialCategoryRepository
    {
        Task<IEnumerable<MaterialCategoryEntity>> GetAllCategories();
        Task<MaterialCategoryEntity> GetById(int id);
        Task<bool> Exists(int id);
        Task<MaterialCategoryEntity> Add(MaterialCategoryEntity category);
        Task<MaterialCategoryEntity> Update(MaterialCategoryEntity category);
        Task Delete(int id);
    }
}
