using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Sectors
{
    public interface ISectorsInputPort
    {
        Task Handle(SectorDTO sector);
        Task GetListSectorsByIdFarm(int idFarm);
        Task GetSectorsByOrderId(int orderId);
    }
}
