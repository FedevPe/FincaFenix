using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class DetailWorkOrderPresenter : IDetailWorkOrderOutputPort
    {
        public bool IsSuccess { get; private set; } = false;

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
