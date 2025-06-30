using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO
{
    public class DetailWorkOrderFormDTO
    {
        public int Id { get; set; }
        public WorkOrderDTO WorkOrder { get; set; }
        public EmployeeDTO Employee { get; set; }
        public List<DetailSectorFarmDTO> SectorList { get; set; }
        public decimal Performance { get; set; }
        public decimal WorkedHours { get; set; }
        public string Description { get; set; }
    }
}
