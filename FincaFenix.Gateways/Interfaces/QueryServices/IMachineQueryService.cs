using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IMachineQueryService
    {
        Task<IEnumerable<MachineEntity>> GetMachinesAsync();
    }
}
