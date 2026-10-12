namespace FincaFenix.Entities.DTOs.TaskDTOs
{
    public class TaskFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public bool IncludeDeleted { get; set; }
    }
}
