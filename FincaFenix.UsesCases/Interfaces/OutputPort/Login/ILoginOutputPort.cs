using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort.Login
{
    public interface ILoginOutputPort
    {
        UserDTO User { get; }
        Task Handle(UserEntity user);
    }
}
