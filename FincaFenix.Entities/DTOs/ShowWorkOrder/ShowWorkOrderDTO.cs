using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.ShowWorkOrder
{
    public class ShowWorkOrderDTO
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal TotalAreaWorked { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TaskDTO Task { get; set; }
        public FarmDTO Farm { get; set; }
        public IEnumerable<DetailWorkOrderDTO.GetDetailWorkOrder.DetailWorkOrderDTO> DetailsWorkOrder { get; set; }
        public IEnumerable<DetailSectorFarmDTO> SectorList { get; set; }
        public RecipeWorkOrderDTO Recipe { get; set; }
        public bool IsDeleted { get; set; }
    }
}