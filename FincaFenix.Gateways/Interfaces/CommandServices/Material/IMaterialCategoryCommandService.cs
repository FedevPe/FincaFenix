using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Material
{
    public interface IMaterialCategoryCommandService
    {
        Task<MaterialCategoryEntity> CreateAsync(MaterialCategoryEntity category);
        Task<MaterialCategoryEntity> UpdateAsync(MaterialCategoryEntity category);
        Task DeleteAsync(int id);
    }
}
