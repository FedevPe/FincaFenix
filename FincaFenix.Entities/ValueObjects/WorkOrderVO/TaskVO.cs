namespace FincaFenix.Entities.ValueObjects.WorkOrderVO
{
    public class TaskVO
    {
        public TaskVO(string descripcion)
        {
            Descripcion = descripcion;
        }

        public string Descripcion { get; }
    }
}
