namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO
{
    public class WorkOrderInfoDTO
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public int FarmId { get; set; }
        public int TaskId { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
