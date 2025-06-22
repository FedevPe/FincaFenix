using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.POCOEntities
{
    public class WorkOrderEntity
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public int RecipeId { get; set; }
        public RecipeEntity Recipe { get; set; }
        public int TaskId { get; set; }
        public TaskEntity Task { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<DetailWorkOrderEntity> DetailWorkOrderList { get; set; } = new List<DetailWorkOrderEntity>();
    }
}
