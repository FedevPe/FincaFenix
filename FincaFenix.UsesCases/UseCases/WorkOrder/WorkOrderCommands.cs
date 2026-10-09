using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository.WorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.WorkOrder;

public record CreateWorkOrderCommand(WorkOrderDTO WorkOrder) : IRequest<OperationResultDTO>;
public record UpdateWorkOrderStateCommand(int WorkOrderId, string NewStatus) : IRequest<bool>;
public record UpdateWorkOrderCommand(UpdateWorkOrderDTO WorkOrder) : IRequest<bool>;

public class CreateWorkOrderHandler(ICreateWorkOrderRepository repository, IMapper mapper) : IRequestHandler<CreateWorkOrderCommand, OperationResultDTO>
{
    public async Task<OperationResultDTO> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<WorkOrderEntity>(request.WorkOrder);
        var id = await repository.CreateWorkOrder(entity);
        return new OperationResultDTO { Success = id > 0 };
    }
}

public class UpdateWorkOrderStateHandler(IUpdateWorkOrderRepository repository) : IRequestHandler<UpdateWorkOrderStateCommand, bool>
{
    public async Task<bool> Handle(UpdateWorkOrderStateCommand request, CancellationToken cancellationToken)
    {
        return await repository.UpdateWorkOrderState(request.WorkOrderId, request.NewStatus);
    }
}

public class UpdateWorkOrderHandler(IUpdateWorkOrderRepository repository, IMapper mapper) : IRequestHandler<UpdateWorkOrderCommand, bool>
{
    public async Task<bool> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var recipe = request.WorkOrder.Recipe != null
            ? mapper.Map<RecipeEntity>(request.WorkOrder.Recipe)
            : null;

        return await repository.UpdateWorkOrderAsync(request.WorkOrder, recipe);
    }
}
