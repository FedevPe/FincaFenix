namespace FincaFenix.Entities.POCOEntities
{
    public class InventoryMovementEntity
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity? Material { get; set; }
        public int FarmId { get; set; }
        public FarmEntity? Farm { get; set; }
        public string? MovementType { get; set; }
        public decimal Amount { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }
        public decimal PreviousStock { get; set; }
        public decimal ResultingStock { get; set; }
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public string? Observations { get; set; }
        public string? Origin { get; set; }
        public int? WorkOrderId { get; set; }
        public WorkOrderEntity? WorkOrder { get; set; }
    }
}
