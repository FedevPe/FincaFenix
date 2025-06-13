using FincaFenix.UserInterface.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace FincaFenix.UserInterface.Layout
{
    public partial class MainLayout
    {
        private DrawerClipMode _clipMode = DrawerClipMode.Never;
        bool _drawerOpen = true;
        private bool _showAddOrderButton = false;

        protected override void OnInitialized(){
            NavManager.LocationChanged += OnLocationChanged;
            TextAppBar.OnChange += StateHasChanged;
            UpdateButtonVisibility(NavManager.Uri);
        }
        private void OnLocationChanged(object? sender, LocationChangedEventArgs e){
            UpdateButtonVisibility(e.Location);
            StateHasChanged();
        }
        private void UpdateButtonVisibility(string uri){
            var relativeUri = NavManager.ToBaseRelativePath(uri).ToLower();
            _showAddOrderButton = relativeUri.StartsWith("ordenes");
        }
        public void Dispose(){
            NavManager.LocationChanged -= OnLocationChanged;
            TextAppBar.OnChange -= StateHasChanged;
        }
        void DrawerToggle(){
            _drawerOpen = !_drawerOpen;
        }
         private Task OpenDialogAsync()
        {
            var options = new DialogOptions { BackgroundClass = "blurry-dialog" };

            return DialogService.ShowAsync<CreateOrderForm>("Nueva órden de trabajo", options);
        }
        MudTheme CustomTheme = new MudTheme(){
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Teal.Darken4,
                AppbarBackground = Colors.Teal.Darken4,
                Secondary = Colors.Teal.Darken3,
                Tertiary = Colors.Gray.Lighten5,
                Info = Colors.BlueGray.Default
            }
        };
    }
}