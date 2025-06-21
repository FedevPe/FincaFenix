using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Sectors
{
    public interface ISectorOutputPort
    {
        public List<SectorDTO> SectorList { get; }
        Task HandleList(IEnumerable<SectorEntity> sectorList);
        Task Handle(SectorEntity sector);
    }
}
