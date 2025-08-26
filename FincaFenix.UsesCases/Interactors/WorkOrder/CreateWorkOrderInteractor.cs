using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.UsesCases.Interactors.WorkOrder
{
    public class CreateWorkOrderInteractor(
        ICreateWorkOrderOutputPort presenter,
        ICreateWorkOrderRepository repository) : ICreateWorkOrderInputPort
    {
        public async Task CreateWorkOrder(WorkOrderDTO workOrder)
        {
            // 1. Mapear el DTO a la entidad completa (incluyendo Recipe y WorkedSectors)
            var workOrderEntity = WorkOrderMapper.ToEntity(workOrder);

            // 2. Persistir la orden de trabajo
            var idWorkOrder = await repository.CreateWorkOrder(workOrderEntity);

            // 3. Presentar el resultado
            await presenter.Handle(idWorkOrder);
        }
    }
}
