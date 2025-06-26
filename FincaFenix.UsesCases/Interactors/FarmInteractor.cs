using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class FarmInteractor(IFarmOutputPort presenter,
        IFarmRepository repository) : IFarmInputPort
    {
        public async Task GetFarmById(int idFarm)
        {
            await presenter.Handle(await repository.GetFarmById(idFarm));
        }
        public async Task GetListFarm()
        {
            await presenter.HandleList(await repository.GetListFarm());
        }
    }
}
