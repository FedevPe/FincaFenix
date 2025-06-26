namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface ITaskInputPort
    {
        Task GetListTask();
        Task GetTaskById(int id);
    }
}
