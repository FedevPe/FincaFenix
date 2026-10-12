using System.ComponentModel.DataAnnotations.Schema;

namespace FincaFenix.Entities.POCOEntities
{
    public class FruitEntity
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public ICollection<FruitVarietyEntity> Varieties { get; set; } = new List<FruitVarietyEntity>();

        [NotMapped]
        public List<int> VarietyIds { get; set; } = new List<int>();
    }
}
