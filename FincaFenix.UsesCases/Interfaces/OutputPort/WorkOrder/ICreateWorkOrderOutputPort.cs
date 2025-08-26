namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder
{
    public interface ICreateWorkOrderOutputPort
    {
        public bool IsSaved { get; }
        Task Handle(int id);
    }
}
