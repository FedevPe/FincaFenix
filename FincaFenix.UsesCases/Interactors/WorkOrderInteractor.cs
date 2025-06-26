using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class WorkOrderInteractor(
        IWorkOrderOutputPort presenter,
        IWorkOrderRepository repository) : IWorkOrderInputPort
    {
        //public async Task Handle(WorkOrderDTO workOrder)
        //{

        //    //await repository.AddWorkOrder(workOrder);
        //    //await presenter.Handle(workOrder);
        //}
        public Task Handle(WorkOrderDTO workOrder)
        {
            throw new NotImplementedException();
        }
    }
}