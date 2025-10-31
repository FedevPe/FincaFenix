using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder
{
    public class ActivityWorkOrderDTO
    {
        public int Id { get; set; }
        public WorkOrderDTO WorkOrder { get; set; }
        public EmployeeDTO Employee { get; set; }
        public DetailSectorFarmDTO Sector { get; set; }
        public decimal Performance { get; set; }
        public decimal WorkedHours { get; set; }
        public string Description { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
