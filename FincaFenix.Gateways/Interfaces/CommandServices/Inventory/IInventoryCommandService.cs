using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Inventory
{
    public interface IInventoryCommandService
    {
        Task<MovementResultDTO> RegisterMovementAsync(InventoryMovementEntity movement, decimal? stockMinimum);
    }
}
