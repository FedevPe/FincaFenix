using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialEntity>> GetAllMaterialByCategoryId(int categoryId);
        Task<IEnumerable<MaterialEntity>> GetAllMaterialByRecipeId(int recipeId);
        Task<MaterialEntity> GetMaterialById(int id);
        Task<(IEnumerable<MaterialEntity> Materials, int TotalCount)> GetMaterialListPaged(MaterialFilterDTO filter);
        Task<IEnumerable<MaterialEntity>> GetMaterialList();
        Task<bool> Exists(int id);
        Task<MaterialEntity> CreateMaterial(MaterialEntity material);
        Task<MaterialEntity> UpdateMaterial(MaterialEntity material);
        Task DeleteMaterial(int id);
    }
}
