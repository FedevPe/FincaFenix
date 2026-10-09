namespace FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs
{
    public class MaterialFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public bool IncludeDeleted { get; set; }
    }
}
