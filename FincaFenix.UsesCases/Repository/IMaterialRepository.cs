using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialEntity>> GetAllMaterial();
        Task<MaterialEntity> GetMaterialByIdAsync(int id);
    }
}
