namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO
{
    public class InfoDetailWorkOrderDTO
    {
        public int SectorWorkedId { get; set; }
        public decimal Performance { get; set; }
        public decimal WorkedHours { get; set; }
        public string? Description { get; set; }
    }
}
