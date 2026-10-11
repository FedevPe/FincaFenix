namespace FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs
{
    public class ReservationItemDTO
    {
        public int ReservationId { get; set; }
        public int WorkOrderId { get; set; }
        public string OrderNum { get; set; }
        public string WorkOrderStatus { get; set; }
        public int FarmId { get; set; }
        public string FarmName { get; set; }
        public decimal ReservedAmount { get; set; }
        public decimal ConsumedAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReleasedDate { get; set; }
    }
}
