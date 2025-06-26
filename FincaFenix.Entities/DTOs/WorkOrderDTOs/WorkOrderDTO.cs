namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class WorkOrderDTO
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public UserDTO User { get; set; }
        public TaskDTO Task { get; set; }
        public List<MaterialOrderDTO> MaterialsList { get; set; }
        public List<DetailSectorFarmDTO> SectorList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool State { get; set; }
        public bool IsDeleted { get; set; }

    }
}
