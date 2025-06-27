namespace FincaFenix.Entities.DTOs.RecipeDTO
{
    public class RecipeWorkOrderDTO
    {
        public int Id { get; set; }
        public string NumRecipe { get; set; }
        public int MachineId { get; set; }
        public decimal Dose { get; set; }
        public string DoseUnit { get; set; }
        public string State { get; set; } = "Activo";
        public bool IsDelete { get; set; } = false;

        public List<DetailRecipeDTO> Details { get; set; } = new();
    }
}
