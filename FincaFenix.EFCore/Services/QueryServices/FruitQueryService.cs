using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FruitQueryService(
        FincaFenixContext context) : IFruitQueryService
    {
        public async Task<IEnumerable<FruitEntity>> GetFruitList()
        {
            return await context.Fruits
                .AsNoTracking()
                .Include(f => f.Varieties)
                .OrderBy(f => f.Description)
                .ToListAsync();
        }

        public async Task<(IEnumerable<FruitEntity> Fruits, int TotalCount)> GetFruitListPaged(FruitFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Fruits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(f => f.Description.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var fruits = await baseQuery
                .OrderBy(f => f.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Include(f => f.Varieties)
                .ToListAsync();

            return (fruits, totalCount);
        }

        public async Task<FruitEntity> GetFruitById(int id)
        {
            return await context.Fruits
                .AsNoTracking()
                .Include(f => f.Varieties)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> FruitExists(int id)
        {
            return await context.Fruits.AnyAsync(f => f.Id == id);
        }
    }
}
