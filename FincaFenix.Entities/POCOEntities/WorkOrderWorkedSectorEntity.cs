namespace FincaFenix.Entities.POCOEntities
{
    public class WorkOrderWorkedSectorEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrderEntity WorkOrder { get; set; }
        public int SectorFarmId { get; set; }
        public DetailSectorFarmEntity SectorFarm { get; set; }
    }
}
