using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MaterialRepository(IMaterialQueryService queryService) : IMaterialRepository
    {
        public async Task<bool> Exists(int id)
        {
            return await queryService.Exists(id);
        }

        public async Task<IEnumerable<MaterialEntity>> GetAllMaterialByCategoryId(int categoryId)
        {
            return await queryService.GetMaterialListByCategoryId(categoryId);
        }

        public Task<IEnumerable<MaterialEntity>> GetMaterialByOrderIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MaterialEntity>> GetMaterialList()
        {
            return await queryService.GetMaterialList();
        }
    }
}
