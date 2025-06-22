namespace FincaFenix.Entities.POCOEntities
{
    public class DiseasePlague_MaterialEntity
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity Material { get; set; }
        public int DiseasePlagueId { get; set; }
        public DiseasePlagueEntity DiseasePlague { get; set; }
    }
}
