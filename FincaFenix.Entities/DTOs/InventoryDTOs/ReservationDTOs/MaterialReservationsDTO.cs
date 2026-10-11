namespace FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs
{
    public class MaterialReservationsDTO
    {
        public int MaterialId { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public string UnitOfMeasure { get; set; }
        public IEnumerable<ReservationItemDTO> Items { get; set; } = [];
    }
}
