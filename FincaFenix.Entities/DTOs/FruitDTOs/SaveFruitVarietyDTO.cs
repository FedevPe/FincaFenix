namespace FincaFenix.Entities.DTOs.FruitDTOs
{
    public class SaveFruitVarietyDTO
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public int? FruitId { get; set; }
        public bool IsOrganic { get; set; }
    }
}
