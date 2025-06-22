namespace FincaFenix.Entities.POCOEntities
{
    public class DiseasePlagueEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<DiseasePlague_MaterialEntity> MaterialDiseasePlagueList { get; set; }

    }
}
