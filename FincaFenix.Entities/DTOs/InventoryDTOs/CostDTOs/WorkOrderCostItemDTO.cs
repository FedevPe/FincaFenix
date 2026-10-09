namespace FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs
{
    public class WorkOrderCostItemDTO
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal PlannedAmount { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public DateTime FrozenDate { get; set; }
    }
}