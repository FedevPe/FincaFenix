namespace FincaFenix.Entities.POCOEntities
{
    public class EmployeeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Employee_FarmEntity> EmployeeList { get; set; } = new List<Employee_FarmEntity>();
    }
}
