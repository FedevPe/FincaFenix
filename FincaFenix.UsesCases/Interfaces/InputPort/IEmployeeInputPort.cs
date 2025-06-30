namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IEmployeeInputPort
    {
        Task GetEmployeeList(int farmId);
    }
}
