using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
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
                .Include(m => m.UnitOfMeasure)
                .Where(m => materialIds.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await context.Materials
                .AsNoTracking()
                .Include(m => m.UnitOfMeasure)
                .Where(m => !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId)
        {
            var materials = await context.Materials
                .AsNoTracking()
                .Include(m => m.UnitOfMeasure)
                .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
                .ToListAsync();

            return materials
                .GroupBy(x => x.ArticleName)
                .Select(g => g.First())
                .ToList();
        }

        public async Task<MaterialEntity> GetMaterialById(int id)
        {
            return await context.Materials
                .AsNoTracking()
                .Include(m => m.Category)
                .Include(m => m.UnitOfMeasure)
                .Include(m => m.Currency)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<(IEnumerable<MaterialEntity> Materials, int TotalCount)> GetMaterialListPaged(MaterialFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Materials.AsQueryable();

            if (!filter.IncludeDeleted)
                baseQuery = baseQuery.Where(m => !m.IsDeleted);

            if (filter.CategoryId.HasValue)
                baseQuery = baseQuery.Where(m => m.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(m =>
                    m.ArticleName.Contains(search) ||
                    m.CommercialName.Contains(search) ||
                    m.CodeSap.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var materials = await baseQuery
                .OrderBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Include(m => m.Category)
                .Include(m => m.UnitOfMeasure)
                .Include(m => m.Currency)
                .ToListAsync();

            return (materials, totalCount);
        }
    }
}
