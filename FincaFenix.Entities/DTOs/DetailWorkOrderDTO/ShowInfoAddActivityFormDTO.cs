using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO
{
    public class ShowInfoAddActivityFormDTO
    {
        public WorkOrderInfoDTO? WorkOrder { get; set; }
        public FarmDTO? Farm { get; set; }
        public TaskDTO? Task { get; set; }
        public IEnumerable<DetailSectorFarmDTO>? SectorList { get; set; }
        public IEnumerable<EmployeeDTO>? EmployeeList { get; set; }
    }
}
