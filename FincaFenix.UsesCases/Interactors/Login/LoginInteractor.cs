using FincaFenix.Entities.DTOs.Login;
using FincaFenix.UsesCases.Interfaces.InputPort.Login;
using FincaFenix.UsesCases.Interfaces.OutputPort.Login;
using FincaFenix.UsesCases.Repository.Login;

namespace FincaFenix.UsesCases.Interactors.Login
{
    public class LoginInteractor(
        ILoginOutputPort presenter,
        ILoginRepository repository) : ILoginInputPort
    {
        public async Task Login(LoginDTO credentials)
        {
            await presenter.Handle(await repository.ValidateCredentials(credentials));
        }
    }
}