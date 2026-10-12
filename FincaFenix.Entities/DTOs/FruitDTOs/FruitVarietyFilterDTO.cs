namespace FincaFenix.Entities.DTOs.FruitDTOs
{
    public class FruitVarietyFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? FruitId { get; set; }
    }
}
