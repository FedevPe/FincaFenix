using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class MaterialQueryService (
        FincaFenixContext context) : IMaterialQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await context.Materials.AnyAsync(m => m.Id == id);   
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await context.Materials.ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId)
        {
           return await context.Materials
                .Where(x => x.CategoryId == categoryId)
                .GroupBy(x => x.ArticleName) // Agrupa por el nombre del material para identificar los duplicados.
                .Select(x => x.First()) // Selecciona el primer elemento de cada grupo.
                .ToListAsync();
        }
    }
}
