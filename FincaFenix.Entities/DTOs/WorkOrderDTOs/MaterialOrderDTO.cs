namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class MaterialOrderDTO
    {
        public int Id { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public string UnitPacking { get; set; }
        public string Brand { get; set; }
    }
}
