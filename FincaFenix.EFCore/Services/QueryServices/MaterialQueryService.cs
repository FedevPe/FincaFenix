using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class MaterialQueryService (
        FincaFenixContext context) : IMaterialQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await context.Materials.AnyAsync(m => m.Id == id);   
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialListByRecipeId(int recipeId)
        {
            var materialIds = await context.DetailRecipes
                .Where(dr => dr.RecipeId == recipeId)
                .Select(dr => dr.MaterialId)
                .Distinct()
                .ToListAsync();

            return await context.Materials
                .AsNoTracking()
                .Include(m => m.Category)
                .Where(m => materialIds.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await context.Materials.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId)
        {
            return await context.Materials
                .Where(x => x.CategoryId == categoryId)
                .GroupBy(x => x.ArticleName)
                .Select(g => g.First())
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
