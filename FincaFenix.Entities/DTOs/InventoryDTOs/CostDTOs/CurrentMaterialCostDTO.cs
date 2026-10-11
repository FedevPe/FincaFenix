namespace FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs
{
    public class CurrentMaterialCostDTO
    {
        public int MaterialId { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? ReferenceCost { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
    }
}