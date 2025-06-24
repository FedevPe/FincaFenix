using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace FincaFenix.UserInterface
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices();
            builder.Services.AddUserInterfaceServices();

            await builder.Build().RunAsync();
        }
    }
}
