namespace FincaFenix.Entities.ValueObjects.WorkOrderVO
{
    public class SectorVO
    {
        public SectorVO(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
