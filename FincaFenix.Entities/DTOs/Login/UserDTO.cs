namespace FincaFenix.Entities.DTOs.Login
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public string Rol { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
