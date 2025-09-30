namespace FincaFenix.Entities.POCOEntities
{
    public class DetailSectorFarmEntity
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public FarmEntity Farm { get; set; } 
        public string SectorName { get; set; }
        public int? VarietyId { get; set; }
        public FruitVarietyEntity Variety { get; set; }
        public int? NumberPlants { get; set; }
        public decimal? Area { get; set; }
        public int Age {  get; set; }

    }
}
