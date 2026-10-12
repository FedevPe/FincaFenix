using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Farms;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Farms
{
    public class FarmCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IFarmCommandService
    {
        public async Task<FarmEntity> CreateAsync(FarmEntity farm)
        {
            await unitOfWork.BeginAsync();
            try
            {
                farm.Id = 0;
                farm.IsDeleted = false;

                await context.Farms.AddAsync(farm);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return farm;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<FarmEntity> UpdateAsync(FarmEntity farm)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Farms.FirstOrDefaultAsync(f => f.Id == farm.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la finca con Id {farm.Id}.");

                existing.Name = farm.Name;

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
                var existing = await context.Farms.FirstOrDefaultAsync(f => f.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la finca con Id {id}.");

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

        public async Task<FarmEntity> ReactivateAsync(int id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Farms.FirstOrDefaultAsync(f => f.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la finca con Id {id}.");

                existing.IsDeleted = false;

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
    }
}