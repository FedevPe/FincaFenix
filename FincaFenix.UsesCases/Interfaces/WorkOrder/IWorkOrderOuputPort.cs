using FincaFenix.UsesCases.Aggregates;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOuputPort
    {
        public bool IsSaved { get; set; }  
        Task Handle(WorkOrderAggregate workOrder);
    }
}
