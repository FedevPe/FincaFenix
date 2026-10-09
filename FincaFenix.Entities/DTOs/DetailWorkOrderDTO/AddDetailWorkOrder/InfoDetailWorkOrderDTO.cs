namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder
{
    public class InfoDetailWorkOrderDTO
    {
        public int SectorWorkedId { get; set; }
        public decimal MachinePasses { get; set; }
        public decimal WorkedHours { get; set; }
        public decimal? ProducedAmount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
