using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Material
{
    public class UnitOfMeasureCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IUnitOfMeasureCommandService
    {
        public async Task<UnitOfMeasureEntity> CreateAsync(UnitOfMeasureEntity unit)
        {
            await unitOfWork.BeginAsync();
            try
            {
                unit.Id = 0;
                unit.IsDeleted = false;

                await context.UnitOfMeasures.AddAsync(unit);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return unit;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UnitOfMeasureEntity> UpdateAsync(UnitOfMeasureEntity unit)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == unit.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la unidad de medida con Id {unit.Id}.");

                existing.Description = unit.Description;

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
                var existing = await context.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la unidad de medida con Id {id}.");

                existing.IsDeleted = true;

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
