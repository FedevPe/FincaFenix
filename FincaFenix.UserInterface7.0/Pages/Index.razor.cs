
using FincaFenix.UserInterface7._0.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Pages
{
    public partial class Index
    {
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private TextAppBarStateService TextAppBar { get; set; }
        public string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TextAppBar.PageTitle = "Menu principal";
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                UserName = user.Identity.Name;
                if (user.IsInRole("operario"))
                {
                    NavigationManager.NavigateTo("/ordenestrabajo");
                }
            }
            else
            {
                UserName = "Invitado";
            }
        }

        private void NavigateToModule(string route)
        {
            try
            {
                NavigationManager.NavigateTo(route);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error al navegar: {ex.Message}", Severity.Error);
            }
        }
    }
}
