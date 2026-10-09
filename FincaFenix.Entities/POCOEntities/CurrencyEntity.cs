namespace FincaFenix.Entities.POCOEntities
{
    public class CurrencyEntity
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public bool IsDeleted { get; set; }
    }
}
