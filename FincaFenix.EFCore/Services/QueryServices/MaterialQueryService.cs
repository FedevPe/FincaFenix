using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class MaterialQueryService : FincaFenixContext, IMaterialQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await Materials.AnyAsync(m => m.Id == id);   
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await Materials.ToListAsync();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId)
        {
            return await Materials.Where(x => x.CategoryId == categoryId).ToListAsync();
        }
    }
}
