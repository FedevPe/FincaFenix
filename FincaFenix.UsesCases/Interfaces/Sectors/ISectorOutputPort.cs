using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Sectors
{
    public interface ISectorOutputPort
    {
        Task HandleList(List<SectorDTO> materialList);
        Task Handle(SectorDTO material);
    }
}
