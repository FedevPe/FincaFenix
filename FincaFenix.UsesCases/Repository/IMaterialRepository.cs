using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMaterialRepository
    {
        Task<IEnumerable<MaterialEntity>> GetAllMaterial();
        Task<IEnumerable<MaterialEntity>> GetMaterialByOrderIdAsync(int id);
    }
}
