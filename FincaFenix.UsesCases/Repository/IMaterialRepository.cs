using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialEntity>> GetAllMaterialByCategoryId(int categoryId);
        Task<IEnumerable<MaterialEntity>> GetMaterialByOrderIdAsync(int id);
        Task<IEnumerable<MaterialEntity>> GetMaterialList();
        Task<bool> Exists(int id);


    }
}
