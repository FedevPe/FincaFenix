using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Fruit
{
    public class FruitCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IFruitCommandService
    {
        public async Task<FruitEntity> CreateAsync(FruitEntity fruit)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var varietyIds = fruit.VarietyIds ?? new List<int>();
                fruit.Id = 0;
                fruit.Varieties = new List<FruitVarietyEntity>();

                await context.Fruits.AddAsync(fruit);
                await unitOfWork.SaveChangesAsync();

                if (varietyIds.Count > 0)
                {
                    var varieties = await context.FruitVarieties
                        .Where(v => varietyIds.Contains(v.Id))
                        .ToListAsync();

                    foreach (var variety in varieties)
                    {
                        variety.FruitId = fruit.Id;
                        fruit.Varieties.Add(variety);
                    }

                    await unitOfWork.SaveChangesAsync();
                }

                await unitOfWork.CommitAsync();

                fruit.VarietyIds = varietyIds;
                return fruit;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<FruitEntity> UpdateAsync(FruitEntity fruit)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var selected = (fruit.VarietyIds ?? new List<int>()).ToHashSet();

                var existing = await context.Fruits
                    .Include(f => f.Varieties)
                    .FirstOrDefaultAsync(f => f.Id == fruit.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la fruta con Id {fruit.Id}.");

                existing.Description = fruit.Description;

                foreach (var variety in existing.Varieties.Where(v => !selected.Contains(v.Id)))
                    variety.FruitId = null;

                var toAssign = await context.FruitVarieties
                    .Where(v => selected.Contains(v.Id) && v.FruitId != existing.Id)
                    .ToListAsync();

                foreach (var variety in toAssign)
                    variety.FruitId = existing.Id;

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                existing.VarietyIds = selected.ToList();
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
                var existing = await context.Fruits.FirstOrDefaultAsync(f => f.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la fruta con Id {id}.");

                var hasVarieties = await context.FruitVarieties.AnyAsync(v => v.FruitId == id);

                if (hasVarieties)
                    throw new BusinessRuleException(
                        $"No se puede eliminar la fruta {id} porque tiene variedades asociadas.");

                context.Fruits.Remove(existing);
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
