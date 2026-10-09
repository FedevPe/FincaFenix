namespace FincaFenix.Entities.POCOEntities
{
    public class ConsumptionEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrderEntity? WorkOrder { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity? Material { get; set; }
        public decimal ConsumedAmount { get; set; }
        public decimal AppliedAmount { get; set; }
        public string? Unit { get; set; }
        public string? Origin { get; set; }
        public decimal? UnitCost { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }
        public DateTime CalculatedDate { get; set; }
        public int? UserId { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
