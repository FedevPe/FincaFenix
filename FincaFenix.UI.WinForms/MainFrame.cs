using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using UniversitarySystem.Views;

namespace UniversitarySystem.UI.WinForms
{
    public partial class MainFrame : Form
    {
        public MainFrame(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            var services = new ServiceCollection();
            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = serviceProvider;
            blazorWebView1.RootComponents.Add<App>("#app");
        }
    }
}
