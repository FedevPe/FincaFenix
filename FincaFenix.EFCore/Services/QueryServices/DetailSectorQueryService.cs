using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class DetailSectorQueryService(
        FincaFenixContext context) : IDetailSectorQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await context.DetailSectors.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByFarmId(int farmId)
        {
            return await context.DetailSectors.Where(ds => ds.FarmId == farmId)
                .Select(ds => new DetailSectorFarmEntity
                {
                    Id = ds.Id,
                    FarmId = ds.FarmId,
                    SectorName = ds.SectorName,
                    VarietyId = ds.VarietyId,
                    NumberPlants = ds.NumberPlants,
                    Area = ds.Area,
                    Age = ds.Age,
                    Variety = ds.Variety != null ? new FruitVarietyEntity
                    {
                        Id = ds.Variety.Id,
                        Description = ds.Variety.Description,
                        Fruit = ds.Variety.Fruit != null ? new FruitEntity
                        {
                            Id = ds.Variety.Fruit.Id,
                            Description = ds.Variety.Fruit.Description
                        } : null
                    } : null
                })
                .OrderBy(ds => ds.Variety.Fruit.Description)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DetailSectorFarmEntity>> GetSectorListByOrderId(int orderId)
        {
            return await context.WorkOrderWorkedSectors.Where(x => x.WorkOrderId == orderId)
                .Select(x => x.SectorFarm)
                .OrderBy(x => x.SectorName)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
