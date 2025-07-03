using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Sectors;

namespace FincaFenix.Presenters.Implementations
{
    public class SectorPresenter : IDetailSectorOutputPort
    {
        public List<DetailSectorFarmDTO>? SectorList { get; private set; }
        public Task Handle(DetailSectorFarmEntity sector)
        {
            throw new NotImplementedException();
        }
        public Task HandleList(IEnumerable<DetailSectorFarmEntity> sectorList)
        {
            SectorList = sectorList.Select(sector => new DetailSectorFarmDTO
            {
                Id = sector.Id,
                FarmId = sector.FarmId,
                SectorName = sector.SectorName,
                VarietyId = sector.VarietyId,
                NumberPlants = sector.NumberPlants
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
