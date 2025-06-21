using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Sectors;

namespace FincaFenix.Presenters.Implementations
{
    public class SectorPresenter : ISectorOutputPort
    {
        public List<SectorDTO>? SectorList { get; private set; }
        public Task Handle(SectorEntity sector)
        {
            throw new NotImplementedException();
        }
        public Task HandleList(IEnumerable<SectorEntity> sectorList)
        {
            SectorList = sectorList.Select(sector => new SectorDTO
            {
                Id = sector.Id,
                Description = sector.Description
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
