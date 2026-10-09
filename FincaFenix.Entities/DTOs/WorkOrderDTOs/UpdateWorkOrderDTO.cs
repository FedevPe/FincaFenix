using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class UpdateWorkOrderDTO
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalArea { get; set; }

        public RecipeWorkOrderDTO? Recipe { get; set; }
    }
}
