using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;

namespace FincaFenix.Gateways.Implementations.DetailWorkOrder
{
    public class AddDetailWorkOrderRepository(
        IDetailWOCommandService command,
        IWorkOrderConsumptionCommand consumptionCommand,
        IUnitOfWork unitOfWork) : IAddDetailWorkOrderRepository
    {
        public async Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var accumulated = await command.GetAccumulatedAreaPercentageAsync(
                    detailWorkOrder.WorkOrderId,
                    detailWorkOrder.SectorWorkedId,
                    forUpdate: true);

                if (accumulated + detailWorkOrder.PercentageAreaWorked > 100m)
                {
                    var remaining = 100m - accumulated;
                    throw new BusinessRuleException(
                        $"El sector ya tiene {accumulated}% de área trabajada; el máximo para esta actividad es {remaining}%.");
                }

                var id = await command.SaveDetailWorkOrderAsync(detailWorkOrder);

                await consumptionCommand.ApplyAutomaticConsumptionAsync(detailWorkOrder.WorkOrderId);

                await unitOfWork.CommitAsync();
                return id;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
