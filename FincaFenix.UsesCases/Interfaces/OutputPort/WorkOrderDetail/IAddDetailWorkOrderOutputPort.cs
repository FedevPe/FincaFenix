namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail
{
    public interface IAddDetailWorkOrderOutputPort
    {
        public bool IsSuccess { get; }
        Task Handle(int detailWOId);
    }
}
