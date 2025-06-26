namespace FincaFenix.Entities.DTOs.RecipeDTO
{
    public class DetailRecipeDTO
    {
        public int CategoryId { get; set; }
        public int MaterialId { get; set; }
        public decimal CantRequerida { get; set; }
        public string UnidadCantRequerida { get; set; }
        public decimal CantEstimada { get; set; }
        public string UnidadCantEstimada { get; set; }
    }
}
