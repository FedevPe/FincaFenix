using FincaFenix.Entities.DTOs.InventoryDTOs;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IWorkOrderConsumptionCommand
    {
        // Debe invocarse dentro de una transacción provista por el llamador.
        Task ApplyAutomaticConsumptionAsync(int workOrderId);

        // Maneja su propia transacción (alta manual).
        Task<ConsumptionResultDTO> RegisterManualConsumptionAsync(RegisterConsumptionDTO dto);
    }
}
