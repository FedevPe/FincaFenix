using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IMaterialQueryService
    {
        Task<IEnumerable<MaterialEntity>> GetMaterialListByCategoryId(int categoryId);
        Task<IEnumerable<MaterialEntity>> GetMaterialList();


    }
}
