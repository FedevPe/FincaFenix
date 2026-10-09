using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Material
{
    public interface IMaterialCommandService
    {
        Task<MaterialEntity> CreateMaterialAsync(MaterialEntity material);
        Task<MaterialEntity> UpdateMaterialAsync(MaterialEntity material);
        Task DeleteMaterialAsync(int id);
    }
}
