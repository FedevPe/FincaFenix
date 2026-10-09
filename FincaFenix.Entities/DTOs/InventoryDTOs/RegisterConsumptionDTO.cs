namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class RegisterConsumptionDTO
    {
        public int WorkOrderId { get; set; }
        public int MaterialId { get; set; }
        public decimal Amount { get; set; }
        public string? Unit { get; set; }
        public string? Observations { get; set; }
        public int? UserId { get; set; }
    }
}
