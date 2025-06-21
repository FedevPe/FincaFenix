using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface ISectorRepository
    {
        Task<IEnumerable<SectorEntity>> GetAllSectorsByFarmId(int farmId);
        Task<IEnumerable<SectorEntity>> GetAllSectorsByOrderId(int orderId);
    }
}
