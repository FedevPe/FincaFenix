using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class UnitOfMeasureQueryService (
        FincaFenixContext context) : IUnitOfMeasureQueryService
    {
        public async Task<IEnumerable<UnitOfMeasureEntity>> GetList()
        {
            return await context.UnitOfMeasures
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

        public async Task<UnitOfMeasureEntity> GetById(int id)
        {
            return await context.UnitOfMeasures
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> Exists(int id)
        {
            return await context.UnitOfMeasures.AnyAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}
