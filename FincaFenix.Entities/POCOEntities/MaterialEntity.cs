namespace FincaFenix.Entities.POCOEntities
{
    public class MaterialEntity
    {
        public int Id { get; set; }
        public string CodeSap { get; set; }
        public string DescriptionSap { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public MaterialCategoryEntity Category { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<DiseasePlague_MaterialEntity> DiseasePlagueMaterialList { get; set; }

    }
}
