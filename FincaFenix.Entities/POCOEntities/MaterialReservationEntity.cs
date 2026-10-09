namespace FincaFenix.Entities.POCOEntities
{
    public class MaterialReservationEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrderEntity? WorkOrder { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity? Material { get; set; }
        public int FarmId { get; set; }
        public FarmEntity? Farm { get; set; }
        public decimal ReservedAmount { get; set; }
        public decimal ConsumedAmount { get; set; }
        public string? State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReleasedDate { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
