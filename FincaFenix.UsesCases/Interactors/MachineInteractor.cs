using FincaFenix.UsesCases.Interfaces.Machine;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class MachineInteractor(
        IMachineOutputPort presenter,
        IMachineRepository repository) : IMachineInputPort
    {
        public async Task GetMachineList()
        {
            await presenter.HandleList(await repository.GetMachineList());
        }
    }
}
