namespace FincaFenix.Entities.POCOEntities
{
    public class MachineEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Capacity { get; set; }
        public string CapacityUnit { get; set; }
        public bool IsDeleted { get; set; }
    }
}
