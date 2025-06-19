namespace FincaFenix.Entities.ValueObjects.WorkOrderVO
{
    public class WorkOrderPeriodVO
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public WorkOrderPeriodVO(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
    }
}
