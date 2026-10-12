using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Fruit
{
    public class FruitVarietyCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IFruitVarietyCommandService
    {
        public async Task<FruitVarietyEntity> CreateAsync(FruitVarietyEntity variety)
        {
            await unitOfWork.BeginAsync();
            try
            {
                variety.Id = 0;
                variety.FruitId = null;
                variety.Fruit = null;

                await context.FruitVarieties.AddAsync(variety);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return variety;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<FruitVarietyEntity> UpdateAsync(FruitVarietyEntity variety)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.FruitVarieties.FirstOrDefaultAsync(v => v.Id == variety.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la variedad con Id {variety.Id}.");

                existing.Description = variety.Description;
                existing.IsOrganic = variety.IsOrganic;

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return existing;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.FruitVarieties.FirstOrDefaultAsync(v => v.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la variedad con Id {id}.");

                var hasSectors = await context.DetailSectors.AnyAsync(d => d.VarietyId == id);

                if (hasSectors)
                    throw new BusinessRuleException(
                        $"No se puede eliminar la variedad {id} porque tiene sectores asociados.");

                context.FruitVarieties.Remove(existing);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
