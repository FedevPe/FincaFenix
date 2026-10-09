namespace FincaFenix.Entities.POCOEntities
{
    public class WorkOrderCostEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrderEntity? WorkOrder { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity? Material { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }
        public decimal PlannedAmount { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime FrozenDate { get; set; }
    }
}
