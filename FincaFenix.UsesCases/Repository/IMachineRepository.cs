using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IMachineRepository
    {
        Task<IEnumerable<MachineEntity>> GetMachineList();
    }
}
