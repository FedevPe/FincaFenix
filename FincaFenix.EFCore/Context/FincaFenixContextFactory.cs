using FincaFenix.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class FincaFenixContextFactory : IDesignTimeDbContextFactory<FincaFenixContext>
{
    public FincaFenixContext CreateDbContext(string[] args)
    {
        var projectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FincaFenix.UserInterface7.0");
        // 1. Opcional: Cargar la configuración de appsettings.json si es necesario.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // 2. Obtener la cadena de conexión.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 3. Crear las opciones del DbContext.
        var optionsBuilder = new DbContextOptionsBuilder<FincaFenixContext>();
        optionsBuilder.UseSqlServer(connectionString);

        // 4. Devolver una nueva instancia del contexto.
        return new FincaFenixContext(optionsBuilder.Options);
    }
}
