using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;

namespace FincaFenix.Presenters.Implementations.WorkOrder
{
    public class UpdateWorkOrderPresenter : IUpdateWorkOrderOutputPort
    {
        public bool IsSuccesUpdate { get; private set; }

        public async Task Handle(bool isSucces)
        {
            IsSuccesUpdate = isSucces;
            await Task.CompletedTask;
        }
    }
}
