using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort.Login;

namespace FincaFenix.Presenters.Implementations.Login
{
    public class LoginPresenter : ILoginOutputPort
    {
        public UserDTO User { get; private set; }

        public async Task Handle(UserEntity user)
        {
            User = new UserDTO()
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                User = user.User,
                Email = user.Email,
                Rol = user.Rol.Description,
                RolId = user.RoleId,
            };
            await Task.CompletedTask;
        }
    }
}
