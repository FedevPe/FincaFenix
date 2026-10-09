using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MaterialCategoryRepository(
        IMaterialCategoryQueryService queryService,
        IMaterialCategoryCommandService commandService) : IMaterialCategoryRepository
    {
        public async Task<bool> Exists(int id)
        {
            return await queryService.Exists(id);
        }

        public async Task<IEnumerable<MaterialCategoryEntity>> GetAllCategories()
        {
            return await queryService.GetCategoriesList();
        }

        public async Task<MaterialCategoryEntity> GetById(int id)
        {
            return await queryService.GetById(id);
        }

        public async Task<MaterialCategoryEntity> Add(MaterialCategoryEntity category)
        {
            return await commandService.CreateAsync(category);
        }

        public async Task<MaterialCategoryEntity> Update(MaterialCategoryEntity category)
        {
            return await commandService.UpdateAsync(category);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }
    }
}
