namespace FincaFenix.Entities.DTOs.FruitDTOs
{
    public class FruitVarietyDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? FruitId { get; set; }
        public string? FruitName { get; set; }
        public bool IsOrganic { get; set; }
    }
}
