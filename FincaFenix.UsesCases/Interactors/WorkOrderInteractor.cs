using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class WorkOrderInteractor(
        IWorkOrderOuputPort presenter,
#pragma warning disable CS9113 // El parámetro no está leído.
        IWorkOrderRepository repository) : IWorkOrderInputPort
#pragma warning restore CS9113 // El parámetro no está leído.
    {
        public async Task Handle(WorkOrderDTO workOrder)
        {
            //Lógica para convertir dto a agregado.
            //await repository.AddWorkOrder(workOrder); 
            await presenter.Handle(workOrder);
        }
    }
}