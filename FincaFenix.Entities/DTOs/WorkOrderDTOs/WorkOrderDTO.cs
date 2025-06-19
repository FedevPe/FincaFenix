namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class WorkOrderDTO
    {
        public int Id { get; set; } 
        public int OrderNum { get; set; }
        public UserDTO User { get; set; } 
        public TaskOrderDTO Task { get; set; }
        public List<MaterialDTO> MaterialsList { get; set; } 
        public List<SectorDTO> SectorList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool State { get; set; }
        public bool IsDeleted { get; set; }

    }
}
