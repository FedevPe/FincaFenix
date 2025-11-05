using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;

namespace FincaFenix.Presenters.Implementations.DetailWorkOrder
{
    public class AddDetailWorkOrderPresenter : IAddDetailWorkOrderOutputPort
    {
        public bool IsSuccess { get; private set; }

        public async Task Handle(int detailWOId)
        {
            if (detailWOId > 0)
            {
                IsSuccess = true;
            }
            await Task.CompletedTask;
        }
    }
}
