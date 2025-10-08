using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;

namespace FincaFenix.Presenters.Implementations.WorkOrder
{
    public class CreateWorkOrderPresenter : ICreateWorkOrderOutputPort
    {
        public bool IsSaved { private set; get; }

        public Task Handle(int id)
        {
            if (id != 0)
                IsSaved = true;

            return Task.CompletedTask;
        }
    }
}
