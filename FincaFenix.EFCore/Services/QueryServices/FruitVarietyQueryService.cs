using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FruitVarietyQueryService(
        FincaFenixContext context) : IFruitVarietyQueryService
    {
        public async Task<IEnumerable<FruitVarietyEntity>> GetVarietyList(int? fruitId)
        {
            var query = context.FruitVarieties
                .AsNoTracking()
                .Include(v => v.Fruit)
                .AsQueryable();

            if (fruitId.HasValue)
                query = query.Where(v => v.FruitId == fruitId.Value);

            return await query
                .OrderBy(v => v.Fruit != null ? v.Fruit.Description : "")
                .ThenBy(v => v.Description)
                .ToListAsync();
        }

        public async Task<(IEnumerable<FruitVarietyEntity> Varieties, int TotalCount)> GetVarietyListPaged(FruitVarietyFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.FruitVarieties.AsQueryable();

            if (filter.FruitId.HasValue)
                baseQuery = baseQuery.Where(v => v.FruitId == filter.FruitId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(v => v.Description.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var varieties = await baseQuery
                .OrderBy(v => v.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Include(v => v.Fruit)
                .ToListAsync();

            return (varieties, totalCount);
        }

        public async Task<FruitVarietyEntity> GetVarietyById(int id)
        {
            return await context.FruitVarieties
                .AsNoTracking()
                .Include(v => v.Fruit)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> VarietyExists(int id)
        {
            return await context.FruitVarieties.AnyAsync(v => v.Id == id);
        }
    }
}
