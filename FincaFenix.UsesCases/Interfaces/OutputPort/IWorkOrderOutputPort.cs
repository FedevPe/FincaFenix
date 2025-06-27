using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOutputPort
    {
        public bool IsSaved { get; }
        Task Handle(int workOrderId);
    }
}
