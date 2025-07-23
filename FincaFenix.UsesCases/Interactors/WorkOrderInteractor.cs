using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using FincaFenix.UsesCases.Repository;
using FincaFenix.Validations.Validators;

namespace FincaFenix.UsesCases.Interactors
{
    public class WorkOrderInteractor(
        IWorkOrderOutputPort presenter,
        IWorkOrderRepository repository) : IWorkOrderInputPort
    {
        public async Task GetWorkOrderById(int id)
        {
            await presenter.Handle(await repository.GetWorkOrderById(id));
        }
        public async Task GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
        {
            var (workOrders, totalCount) = await repository.GetWorkOrderList(pageNumber, pageSize, status);
            await presenter.HandleList(workOrders, totalCount);
        }
        public async Task Handle(WorkOrderDTO workOrder)
        {
            // 1. Mapear el DTO a la entidad completa (incluyendo Recipe y WorkedSectors)
            var workOrderEntity = WorkOrderMapper.ToEntity(workOrder);

            // 2. Validar la entidad mapeada
            WorkOrderValidator.Validate(workOrderEntity);

            // 3. Persistir la orden de trabajo
            var idWorkOrder = await repository.AddWorkOrder(workOrderEntity);

            // 4. Presentar el resultado
            await presenter.Handle(idWorkOrder);
        }
    }
}
