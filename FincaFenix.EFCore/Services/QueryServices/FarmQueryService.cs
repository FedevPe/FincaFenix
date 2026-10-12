using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class FarmQueryService(
        FincaFenixContext context) : IFarmQueryService
    {
        public Task<bool> Exists(int id)
        {
            return context.Farms.AnyAsync(f => f.Id == id);
        }

        public Task<FarmEntity> GetFarmById(int farmId)
        {
            return context.Farms
                .Where(f => f.Id == farmId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException($"Farm with ID {farmId} not found.");
        }

        public async Task<IEnumerable<FarmEntity>> GetFarmList()
        {
            return await context.Farms.Where( f => !f.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<(IEnumerable<FarmEntity> Farms, int TotalCount)> GetFarmListPaged(FarmFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Farms.AsQueryable();

            if (!filter.IncludeDeleted)
                baseQuery = baseQuery.Where(f => !f.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(f => f.Name.Contains(search));
            }

            var totalCount = await baseQuery.CountAsync();

            var farms = await baseQuery
                .OrderBy(f => f.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (farms, totalCount);
        }
    }
}
