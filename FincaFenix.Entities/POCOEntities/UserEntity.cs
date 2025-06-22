namespace FincaFenix.Entities.POCOEntities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }
        public RolEntity Rol { get; set; }
    }
}
