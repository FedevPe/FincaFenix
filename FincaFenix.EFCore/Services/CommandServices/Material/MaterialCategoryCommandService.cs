using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Material
{
    public class MaterialCategoryCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IMaterialCategoryCommandService
    {
        public async Task<MaterialCategoryEntity> CreateAsync(MaterialCategoryEntity category)
        {
            await unitOfWork.BeginAsync();
            try
            {
                category.Id = 0;

                await context.MaterialCategories.AddAsync(category);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return category;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<MaterialCategoryEntity> UpdateAsync(MaterialCategoryEntity category)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.MaterialCategories.FirstOrDefaultAsync(c => c.Id == category.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la categoría de material con Id {category.Id}.");

                existing.Description = category.Description;

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
                var existing = await context.MaterialCategories.FirstOrDefaultAsync(c => c.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la categoría de material con Id {id}.");

                var hasMaterials = await context.Materials.AnyAsync(m => m.CategoryId == id);

                if (hasMaterials)
                    throw new BusinessRuleException(
                        $"No se puede eliminar la categoría {id} porque tiene materiales asociados.");

                context.MaterialCategories.Remove(existing);
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
