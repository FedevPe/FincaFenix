namespace FincaFenix.Entities.DTOs.WorkOrderDTOs
{
    public class MaterialRecipeDTO
    {
        public int Id { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public MaterialCategoryDTO Category { get; set; }
        public string Brand { get; set; }
        public string DiseasePlague { get; set; }
        public string CodeSAP { get; set; }
        public string DescriptionSAP { get; set; }
    }
}
