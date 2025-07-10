namespace FincaFenix.Entities.POCOEntities
{
    public class DetailWorkOrderEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrderEntity WorkOrder { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }
        public int SectorWorkedId { get; set; }
        public DetailSectorFarmEntity SectorWorked { get; set; }
        public decimal Performance { get; set; }
        public decimal WorkedHours { get; set; }
        public string? Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
