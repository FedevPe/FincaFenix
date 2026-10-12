namespace FincaFenix.Entities.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string RoleName { get; set; }
    }
}