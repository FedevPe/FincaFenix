using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class MaterialCategoryQueryService : FincaFenixContext, IMaterialCategoryQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await MaterialCategories.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<MaterialCategoryEntity>> GetCategoriesList()
        {
            return await MaterialCategories.ToListAsync();
        }
    }
}
