namespace FincaFenix.UsesCases.POCOEntities
{
    public class WorkOrderEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int MaterialListId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool State { get; set; }
        public bool IsDeleted { get; set; }
    }
}
