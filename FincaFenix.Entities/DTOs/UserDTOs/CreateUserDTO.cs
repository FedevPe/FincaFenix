namespace FincaFenix.Entities.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}