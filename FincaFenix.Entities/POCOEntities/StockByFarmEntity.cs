namespace FincaFenix.Entities.POCOEntities
{
    public class StockByFarmEntity
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity? Material { get; set; }
        public int FarmId { get; set; }
        public FarmEntity? Farm { get; set; }
        public decimal StockFisico { get; set; }
        public decimal StockReservado { get; set; }
        public decimal StockMinimo { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
