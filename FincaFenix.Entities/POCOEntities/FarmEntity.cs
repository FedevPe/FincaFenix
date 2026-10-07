namespace FincaFenix.Entities.POCOEntities
{
    public class FarmEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Employee_FarmEntity> FarmList { get; set; } = new List<Employee_FarmEntity>();
    }
}
