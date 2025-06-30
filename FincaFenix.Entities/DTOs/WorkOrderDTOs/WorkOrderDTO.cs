
using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class WorkOrderDTO
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int FarmId { get; set; }
        public string Description { get; set; }
        public List<MaterialOrderDTO> MaterialsList { get; set; }
        public List<DetailSectorFarmDTO> SectorList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string State { get; set; } = "Activo";
        public bool IsDeleted { get; set; }

        public RecipeWorkOrderDTO Recipe { get; set; }

    }
}
