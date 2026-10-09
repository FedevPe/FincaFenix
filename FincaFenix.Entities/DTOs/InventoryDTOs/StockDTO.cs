namespace FincaFenix.Entities.DTOs.InventoryDTOs
{
    public class StockDTO
    {
        public int MaterialId { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public int FarmId { get; set; }
        public string FarmName { get; set; }
        public decimal StockFisico { get; set; }
        public decimal StockReservado { get; set; }
        public decimal StockDisponible { get; set; }
        public decimal StockMinimo { get; set; }
        public bool IsLow { get; set; }
    }
}
