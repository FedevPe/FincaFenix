using FincaFenix.EFCore.Context;
using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaFenix.EFCore.Services.Material
{
    public class MaterialQueryService : FincaFenixContext, IMaterialQueryService
    {
        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await Materials.ToListAsync();
        }
    }
}
