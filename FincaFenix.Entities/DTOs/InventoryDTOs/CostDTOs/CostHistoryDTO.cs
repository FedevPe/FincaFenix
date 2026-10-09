namespace FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs
{
    public class CostHistoryDTO
    {
        public int MaterialId { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? ReferenceCost { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public ICollection<CostHistoryItemDTO> Items { get; set; }
    }
}