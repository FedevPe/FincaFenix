namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class RegisterMovementDTO
    {
        public int MaterialId { get; set; }
        public int FarmId { get; set; }
        public string MovementType { get; set; }
        public decimal Amount { get; set; }
        public decimal? UnitCost { get; set; }
        public int CurrencyId { get; set; }
        public decimal? StockMinimum { get; set; }
        public string? Observations { get; set; }
        public int? UserId { get; set; }
    }
}
