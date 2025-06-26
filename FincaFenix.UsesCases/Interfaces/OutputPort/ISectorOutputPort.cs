using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Sectors
{
    public interface ISectorOutputPort
    {
        public List<DetailSectorFarmDTO> SectorList { get; }
        Task HandleList(IEnumerable<DetailSectorFarmEntity> sectorList);
        Task Handle(DetailSectorFarmEntity sector);
    }
}
