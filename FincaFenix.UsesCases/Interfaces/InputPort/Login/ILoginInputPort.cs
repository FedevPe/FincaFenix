using FincaFenix.Entities.DTOs.Login;

namespace FincaFenix.UsesCases.Interfaces.InputPort.Login
{
    public interface ILoginInputPort
    {
        Task Login(LoginDTO credentials);
    }
}
