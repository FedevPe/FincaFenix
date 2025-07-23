namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class DetailSectorFarmDTO
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public string SectorName { get; set; }
        public int? VarietyId { get; set; }
        public string VarietyName { get; set; }
        public string FruitName { get; set; }
        public int? NumberPlants { get; set; }
        public decimal? Area { get; set; }
        public bool Selected { get; set; } = false;
    }
}
