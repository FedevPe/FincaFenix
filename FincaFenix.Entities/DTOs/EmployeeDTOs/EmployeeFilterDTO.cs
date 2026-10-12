namespace FincaFenix.Entities.DTOs.EmployeeDTOs
{
    public class EmployeeFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public bool IncludeDeleted { get; set; }
        public int? FarmId { get; set; }
    }
}