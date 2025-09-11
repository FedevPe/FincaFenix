namespace FincaFenix.Entities.POCOEntities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }
        public RolEntity Rol { get; set; }
    }
}
