using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs
{
    public class MaterialDTO
    {
        public int Id { get; set; }
        public string CodeSap { get; set; }
        public string DescriptionSap { get; set; }
        public string ArticleName { get; set; }
        public string CommercialName { get; set; }
        public int CategoryId { get; set; }
        public MaterialCategoryDTO Category { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? ReferenceCost { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
