namespace FincaFenix.Entities.POCOEntities
{
    public class FruitVarietyEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int FruitId { get; set; }
        public FruitEntity Fruit { get; set; }
    }
}
