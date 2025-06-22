using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.POCOEntities
{
    public class DetailRecipeEntity
    {
        public int Id { get; set; }
        public int RecipeId{ get; set; }
        public RecipeEntity Recipe { get; set; }
        public int MaterialId { get; set; }
        public MaterialEntity Material { get; set; }
        public decimal AmountRequired { get; set; }
        public string AmountRequiredUnit { get; set; }
        public decimal EstimatedAmount { get; set; }
        public string EstimatedAmountUnit { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
