using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MaterialRepository(
        IMaterialQueryService queryService,
        IMaterialCommandService commandService) : IMaterialRepository
    {
        public async Task<bool> Exists(int id)
        {
            return await queryService.Exists(id);
        }

        public async Task<IEnumerable<MaterialEntity>> GetAllMaterialByCategoryId(int categoryId)
        {
            return await queryService.GetMaterialListByCategoryId(categoryId);
        }

        public async Task<IEnumerable<MaterialEntity>> GetAllMaterialByRecipeId(int recipeId)
        {
            return await queryService.GetMaterialListByRecipeId(recipeId);
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await queryService.GetMaterialList();
        }

        public async Task<MaterialEntity> GetMaterialById(int id)
        {
            return await queryService.GetMaterialById(id);
        }

        public async Task<(IEnumerable<MaterialEntity> Materials, int TotalCount)> GetMaterialListPaged(MaterialFilterDTO filter)
        {
            return await queryService.GetMaterialListPaged(filter);
        }

        public async Task<MaterialEntity> CreateMaterial(MaterialEntity material)
        {
            return await commandService.CreateMaterialAsync(material);
        }

        public async Task<MaterialEntity> UpdateMaterial(MaterialEntity material)
        {
            return await commandService.UpdateMaterialAsync(material);
        }

        public async Task DeleteMaterial(int id)
        {
            await commandService.DeleteMaterialAsync(id);
        }
    }
}
