using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IWorkOrderInventoryCommand
    {
        Task RegisterReservationsAndCostsAsync(WorkOrderEntity workOrder, ICollection<DetailRecipeEntity> details);

        // Libera las reservas activas no consumidas. No altera el stock físico. Debe invocarse dentro de una transacción.
        Task ReleaseReservationsAsync(int workOrderId);
    }
}
