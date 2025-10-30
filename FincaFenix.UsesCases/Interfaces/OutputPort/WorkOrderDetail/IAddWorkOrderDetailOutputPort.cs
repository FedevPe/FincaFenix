namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail
{
    public interface IAddWorkOrderDetailOutputPort
    {
        public bool IsSuccess { get; }
        Task Handle(int detailWOId);
    }
}
