using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.POCOEntities
{
    public class RecipeEntity
    {
        public int Id { get; set; }
        public string NumRecipe { get; set; }
        public int MachineId { get; set; }
        public MachineEntity Machine { get; set; }
        public decimal Dosage { get; set; }
        public string DosageUnit { get; set; }
        public string State { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<DetailRecipeEntity> DetailRecipeList { get; set; } = new List<DetailRecipeEntity>();
    }
}
