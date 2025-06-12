using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UniversitarySystem.UI.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ConfigureApplication();

            var services = ConfigureServices();

            var servicesProvider = services.BuildServiceProvider();

            Application.Run(new MainFrame(servicesProvider));
        }
        private static void ConfigureApplication()
        {
            ApplicationConfiguration.Initialize();
        }

        private static IServiceCollection ConfigureServices()
        {            
            var services = new ServiceCollection();
            
            //reloadOnChange = se actualiza en caso de que cambie, mientras la aplicación se
            //esta ejecutando
            var configuration = new ConfigurationBuilder()
                //Este metodo define la dirección en la que se buscarán de los archivos de configuración.
                //AppContext.BaseDirectory = es una propiedad de .NET que retorna la ruta del directorio base
                //donde se ejecuta la aplicación. Por ejemplo: C:\Projects\MyApp\bin\Debug\net6.0\
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            services.AddWindowsFormsBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();

            //Esta configuración se instancia durante el ciclo de vida de la aplicación y
            //será compartida por todos los componentes que la necesiten
            services.AddSingleton<IConfiguration>(configuration);

            //services.AddDatabaseServices(configuration);

            return services;
        }
    }
}