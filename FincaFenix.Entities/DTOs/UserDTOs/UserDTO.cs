namespace FincaFenix.Entities.DTOs.UserDTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}