using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IMaterialCategoryQueryService
    {
        Task<IEnumerable<MaterialCategoryEntity>> GetCategoriesList();
    }
}
