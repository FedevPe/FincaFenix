using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.Material
{
    public interface IMaterialQueryService
    {
        Task<IEnumerable<MaterialEntity>> GetMaterialList();
    }
}
