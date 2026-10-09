namespace FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs
{
    public class CreateMaterialDTO
    {
        public string? CodeSap { get; set; }
        public string? DescriptionSap { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public int UnitOfMeasureId { get; set; }
        public decimal? ReferenceCost { get; set; }
        public int CurrencyId { get; set; } = 1;
    }
}
