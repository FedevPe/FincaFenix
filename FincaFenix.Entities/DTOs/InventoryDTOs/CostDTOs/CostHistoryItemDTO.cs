namespace FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs
{
    public class CostHistoryItemDTO
    {
        public int MovementId { get; set; }
        public string MovementType { get; set; }
        public string Origin { get; set; }
        public decimal Amount { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public int FarmId { get; set; }
        public string FarmName { get; set; }
        public DateTime Date { get; set; }
    }
}