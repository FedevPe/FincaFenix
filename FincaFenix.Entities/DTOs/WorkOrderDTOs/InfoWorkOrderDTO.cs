namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class InfoWorkOrderDTO
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public TaskDTO TaskOrder { get; set; }
        public FarmDTO FarmOrder { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; }
        public IEnumerable<DetailSectorFarmDTO> RelationatedSector { get; set; } = new List<DetailSectorFarmDTO>();

    }
}
