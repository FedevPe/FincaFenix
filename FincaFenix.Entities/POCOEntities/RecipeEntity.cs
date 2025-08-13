using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.POCOEntities
{
    public class RecipeEntity
    {
        public int Id { get; set; }
        public string NumRecipe { get; set; }
        public int MachineId { get; set; }
        public MachineEntity Machine { get; set; }
        public decimal TRV { get; set; }
        public decimal VolumeMachine { get; set; }
        public string VolumeMachineUnit { get; set; }
        public string State { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp] public byte[] RowVersion { get; set; }

        public ICollection<DetailRecipeEntity> DetailRecipeList { get; set; } = new List<DetailRecipeEntity>();
    }
}
