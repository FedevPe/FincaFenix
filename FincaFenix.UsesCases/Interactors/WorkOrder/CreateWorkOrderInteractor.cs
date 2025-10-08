using FincaFenix.Entities.DTOs.Validation;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;
using FincaFenix.Validators.Validators.WorkOrder;

namespace FincaFenix.UsesCases.Interactors.WorkOrder
{
    public class CreateWorkOrderInteractor(
        ICreateWorkOrderOutputPort presenter,
        ICreateWorkOrderRepository repository,
        WorkOrderValidator validator) : ICreateWorkOrderInputPort
    {
        public async Task CreateWorkOrder(WorkOrderDTO workOrder)
        {
            var result = await validator.ValidateAsync(workOrder);

            if (result.IsValid)
            {
                // 1. Mapear el DTO a la entidad completa (incluyendo Recipe y WorkedSectors)
                var workOrderEntity = WorkOrderMapper.ToEntity(workOrder);

                // 2. Persistir la orden de trabajo
                var idWorkOrder = await repository.CreateWorkOrder(workOrderEntity);

                // 3. Presentar el resultado
                await presenter.Handle(idWorkOrder);
            }
            else
            {
                await presenter.HandleErrors(result.Errors
                    .Select(error => new ValidationDTO
                    {
                        PropertyName = error.PropertyName,
                        ErrorMessage = error.ErrorMessage,
                    }));
            }

        }
    }
}
