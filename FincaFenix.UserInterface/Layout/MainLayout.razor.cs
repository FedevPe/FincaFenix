using FincaFenix.UserInterface.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface.Layout
{
    public partial class MainLayout
    {
        private DrawerClipMode _clipMode = DrawerClipMode.Never;
        bool _drawerOpen = true;
        [Inject] public TextAppBarState TextAppBar { get; set; }

        protected override void OnInitialized()
        {
            TextAppBar.OnChange += StateHasChanged;
        }
        public void Dispose()
        {

            TextAppBar.OnChange -= StateHasChanged;
        }
        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
        MudTheme CustomTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Teal.Darken4,
                AppbarBackground = Colors.Teal.Darken4,
                Secondary = Colors.Teal.Darken3,
                Tertiary = Colors.Gray.Lighten5,
                Info = Colors.BlueGray.Default,
                Error= Colors.Red.Lighten1
            }
        };
    }
}