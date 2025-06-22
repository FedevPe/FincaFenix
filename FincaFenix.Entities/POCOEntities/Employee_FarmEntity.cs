namespace FincaFenix.Entities.POCOEntities
{
    public class Employee_FarmEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; }
        public int FarmId { get; set; }
        public FarmEntity Farm { get; set; }

    }
}
