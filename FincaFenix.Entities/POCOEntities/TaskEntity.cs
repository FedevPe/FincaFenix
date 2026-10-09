using FincaFenix.Entities.Enum;

namespace FincaFenix.Entities.POCOEntities
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public RendimientoModeEnum RendimientoMode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
