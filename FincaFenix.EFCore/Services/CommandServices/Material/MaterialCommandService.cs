using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Material;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Material
{
    public class MaterialCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IMaterialCommandService
    {
        public async Task<MaterialEntity> CreateMaterialAsync(MaterialEntity material)
        {
            await unitOfWork.BeginAsync();
            try
            {
                await ValidateReferencesAsync(material);

                material.Id = 0;
                material.IsDeleted = false;

                await context.Materials.AddAsync(material);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return material;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<MaterialEntity> UpdateMaterialAsync(MaterialEntity material)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Materials.FirstOrDefaultAsync(m => m.Id == material.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró el material con Id {material.Id}.");

                await ValidateReferencesAsync(material);

                existing.CodeSap = material.CodeSap;
                existing.DescriptionSap = material.DescriptionSap;
                existing.ArticleName = material.ArticleName;
                existing.CommercialName = material.CommercialName;
                existing.CategoryId = material.CategoryId;
                existing.Brand = material.Brand;
                existing.Description = material.Description;
                existing.UnitOfMeasureId = material.UnitOfMeasureId;
                existing.ReferenceCost = material.ReferenceCost;
                existing.CurrencyId = material.CurrencyId;

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

        public async Task DeleteMaterialAsync(int id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Materials.FirstOrDefaultAsync(m => m.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró el material con Id {id}.");

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

        private async Task ValidateReferencesAsync(MaterialEntity material)
        {
            var categoryExists = await context.MaterialCategories.AnyAsync(c => c.Id == material.CategoryId);

            if (!categoryExists)
                throw new NotFoundException($"No se encontró la categoría de material con Id {material.CategoryId}.");

            var unitExists = await context.UnitOfMeasures
                .AnyAsync(u => u.Id == material.UnitOfMeasureId && !u.IsDeleted);

            if (!unitExists)
                throw new NotFoundException($"No se encontró la unidad de medida con Id {material.UnitOfMeasureId}.");

            var currencyExists = await context.Currencies.AnyAsync(c => c.Id == material.CurrencyId);

            if (!currencyExists)
                throw new NotFoundException($"No se encontró la divisa con Id {material.CurrencyId}.");
        }
    }
}
