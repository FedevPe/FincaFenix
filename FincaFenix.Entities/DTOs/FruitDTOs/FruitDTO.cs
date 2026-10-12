namespace FincaFenix.Entities.DTOs.FruitDTOs
{
    public class FruitDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<int> VarietyIds { get; set; } = new List<int>();
        public List<string> VarietyNames { get; set; } = new List<string>();
    }
}
