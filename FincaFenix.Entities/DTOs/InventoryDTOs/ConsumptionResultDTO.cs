namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class ConsumptionResultDTO
    {
        public int ConsumptionId { get; set; }
        public decimal ConsumedAmount { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal ExcessAmount { get; set; }
        public string? UnitOfMeasure { get; set; }
    }
}
