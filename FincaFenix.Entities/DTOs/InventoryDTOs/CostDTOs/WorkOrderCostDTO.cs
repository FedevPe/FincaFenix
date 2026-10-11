namespace FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs
{
    public class WorkOrderCostDTO
    {
        public int WorkOrderId { get; set; }
        public string OrderNum { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalRealCost { get; set; }
        public ICollection<WorkOrderCostItemDTO> Items { get; set; }
    }
}