namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class MovementResultDTO
    {
        public int MovementId { get; set; }
        public decimal PreviousStock { get; set; }
        public decimal ResultingStock { get; set; }
    }
}
