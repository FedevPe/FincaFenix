namespace FincaFenix.Entities.POCOEntities
{
    public class MaterialEntity
    {
        public int Id { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public MaterialCategoryEntity Category { get; set; }
        public string PackingUnit { get; set; }
        public string Marca { get; set; }
        public bool IsDeleted { get; set; }


        public ICollection<DiseasePlague_MaterialEntity> DiseasePlagueMaterialList { get; set; }

    }
}
