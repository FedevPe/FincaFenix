// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
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
        [Inject] private TextAppBarStateService TextAppBar {  get; set; }
        public string UserName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TextAppBar.PageTitle = "Menu principal";
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                // Por defecto Identity guarda el email en ClaimTypes.Name
                UserName = user.Identity.Name;

                // O podés buscar claims específicos:
                // var email = user.FindFirst(ClaimTypes.Email)?.Value;
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

    // Clases para los datos del dashboard
    public class FincaStats
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class TendenciaMensual
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class ActividadSemanal
    {
        public string Operario { get; set; } = string.Empty;
        public int[] Actividades { get; set; } = new int[5]; // Lun-Vie
    }
}
