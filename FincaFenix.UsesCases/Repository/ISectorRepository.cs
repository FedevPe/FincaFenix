using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface ISectorRepository
    {
        Task<SectorEntity> GetSectorById(int id);
        Task<IEnumerable<SectorEntity>> GetAllSectors();
    }
}
