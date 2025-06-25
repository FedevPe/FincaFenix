using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.Material;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MaterialRepository(IMaterialQueryService queryService) : IMaterialRepository
    {
        public async Task<IEnumerable<MaterialEntity>> GetAllMaterial()
        {
            return await queryService.GetMaterialList();
        }

        public Task<IEnumerable<MaterialEntity>> GetMaterialByOrderIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
