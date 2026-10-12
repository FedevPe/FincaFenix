namespace FincaFenix.Entities.DTOs.UserDTOs
{
    public class UserFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public bool IncludeInactive { get; set; }
    }
}