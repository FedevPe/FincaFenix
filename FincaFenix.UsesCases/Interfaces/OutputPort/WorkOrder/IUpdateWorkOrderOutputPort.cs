namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder
{
    public interface IUpdateWorkOrderOutputPort
    {
        public bool IsSuccesUpdate { get; }
        Task Handle();
    }
}
