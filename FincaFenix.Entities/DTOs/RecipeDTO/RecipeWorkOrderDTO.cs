namespace FincaFenix.Entities.DTOs.RecipeDTO
{
    public class RecipeWorkOrderDTO
    {
        public int Id { get; set; }
        public string NumRecipe { get; set; }
        public int MachineId { get; set; }
        public decimal VolumeMachine { get; set; }
        public string VolumeMachineUnit { get; set; }
        public decimal TRV { get; set; } = 0;
        public string Status { get; set; } = "Activo";
        public bool IsDelete { get; set; } = false;

        public List<DetailRecipeDTO> Details { get; set; } = new();
    }
}
