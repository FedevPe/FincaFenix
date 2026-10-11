namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class InventoryMovementDTO
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public int FarmId { get; set; }
        public string FarmName { get; set; }
        public string MovementType { get; set; }
        public string Origin { get; set; }
        public decimal Amount { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal ResultingStock { get; set; }
        public DateTime Date { get; set; }
        public string Observations { get; set; }
        public int? WorkOrderId { get; set; }
        public string OrderNum { get; set; }
    }
}
