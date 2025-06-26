using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MaterialCategoryRepository (IMaterialCategoryQueryService service) : IMaterialCategoryRepository
    {
        public async Task<IEnumerable<MaterialCategoryEntity>> GetAllCategories()
        {
            return await service.GetCategoriesList();
        }
    }
}
