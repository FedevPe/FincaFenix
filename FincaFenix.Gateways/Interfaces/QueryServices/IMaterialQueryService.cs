using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IMaterialQueryService
    {
        Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId);
        Task<IEnumerable<MaterialEntity>> GetMaterialListByRecipeId(int recipeId);
        Task<IEnumerable<MaterialEntity>> GetMaterialList();
        Task<MaterialEntity> GetMaterialById(int id);
        Task<(IEnumerable<MaterialEntity> Materials, int TotalCount)> GetMaterialListPaged(MaterialFilterDTO filter);
        Task<bool> Exists(int id);
    }
}
