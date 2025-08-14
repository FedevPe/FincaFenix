namespace FincaFenix.Entities.DTOs.RecipeDTO
{
    public class MachineRecipeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Capacity { get; set; }
        public string CapacityUnit { get; set; } = "lts";
        public decimal TRV { get; set; } = 0;   
    }
}
