using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.Entities.DTOs.RecipeDTO
{
    public class DetailRecipeDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int MaterialId { get; set; }
        public MaterialRecipeDTO Material { get; set; }
        public decimal AmountRequired { get; set; }
        public string AmountRequiredUnit { get; set; }
        public decimal EstimatedAmount { get; set; } = 0;
        public string EstimatedAmountUnit { get; set; }
        public string Brand { get; set; }
        public string PestDisease { get; set; }
    }
}
