namespace FincaFenix.UsesCases.BusinessObject.WorkOrderVO
{
    public class WorkOrderPeriodVO
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public WorkOrderPeriodVO(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.");
            StartDate = start;
            EndDate = end;
        }
    }
}
